using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading.Channels;
using HubConnection = Microsoft.AspNetCore.SignalR.Client.HubConnection;

namespace SignalRClient
{
    /// <summary>
    /// SignalR通信组件
    /// </summary>
    public class SignalRComponentUpgrade : IDisposable
    {
        private readonly ILog? Log;
        private HubConnection? _connection; // SignalR连接实例
        private readonly string _hubUrl; // SignalR Hub 的 URL
        private readonly TimeSpan _reconnectInterval = TimeSpan.FromSeconds(2); // 重连间隔时间
        private readonly Channel<MessageRequest> _sendQueue; // 异步消息队列
        private readonly ConcurrentQueue<MessageRequest> _pendingMessages; // 待发送消息缓存
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(); // 取消令牌

        public event Action<string>? OnReceiveMessage; // 自定义事件，用于接收消息
        private readonly ConcurrentDictionary<string, Action<string>> _eventHandlers = new ConcurrentDictionary<string, Action<string>>();

        /// <summary>
        /// 构造函数，初始化 Hub URL
        /// </summary>
        /// <param name="hubUrl">SignalR Hub 的 URL</param>
        /// <param name="log">日志实例</param>
        public SignalRComponentUpgrade(string hubUrl, ILog? log = null)
        {
            _hubUrl = hubUrl;
            Log = log;

            // 创建无界队列用于异步发送消息
            _sendQueue = Channel.CreateUnbounded<MessageRequest>(new UnboundedChannelOptions { SingleWriter = false });

            // 用于存储连接中断时的待发送消息
            _pendingMessages = new ConcurrentQueue<MessageRequest>();

            // 启动后台发送任务
            _ = StartSendingBackgroundTask(_cts.Token);
        }

        /// <summary>
        /// 异步启动连接
        /// </summary>
        public async Task<bool> StartAsync()
        {
            // 创建 Hub 连接并配置自动重连策略
            _connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect(new CustomRetryPolicy())
                .Build();

            // 注册连接状态变化事件处理程序
            _connection.Reconnecting += OnReconnecting;
            _connection.Reconnected += OnReconnected;
            _connection.Closed += OnClosed;
            _connection.ServerTimeout = TimeSpan.FromSeconds(10);

            // 注册所有已注册的事件处理器
            foreach (var handler in _eventHandlers)
            {
                _connection.On<string>(handler.Key, handler.Value);
            }

            return await ConnectWithRetryAsync(3);
        }

        /// <summary>
        /// 同步启动连接（阻塞调用线程）
        /// </summary>
        public bool Start()
        {
            return StartAsync().GetAwaiter().GetResult(); // 阻塞当前线程直到异步操作完成
        }

        /// <summary>
        /// 停止连接
        /// </summary>
        public void Stop()
        {
            _connection?.StopAsync().Wait();
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="handler">事件处理器</param>
        public void On(string methodName, Action<string> handler)
        {
            RegisterEvent(methodName, handler);
            _eventHandlers[methodName] = handler;
        }

        /// <summary>
        /// 注册事件到 HubConnection
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="handler">事件处理器</param>
        private void RegisterEvent(string methodName, Action<string> handler)
        {
            if (_connection != null && _connection.State == HubConnectionState.Connected)
            {
                _connection.On<string>(methodName, (msg) =>
                {
                    Log?.Info($"收到消息: {msg} 通过方法: {methodName}");
                    handler(msg);
                });
            }
        }

        /// <summary>
        /// 尝试建立连接，并在失败时重试
        /// </summary>
        private async Task<bool> ConnectWithRetryAsync(int retryTimes = int.MaxValue)
        {
            try
            {
                if (_connection != null)
                {
                    Log?.Info("正在连接SignalR服务...");
                    await _connection.StartAsync();
                }
                Log?.Info("连接SignalR服务成功.");
                return true;
            }
            catch (Exception ex)
            {
                Log?.Error($"连接SignalR服务失败: {ex}");

                if (retryTimes > 0)
                {
                    await Task.Delay(_reconnectInterval);
                    return await ConnectWithRetryAsync(retryTimes - 1); // 如果启动失败，继续尝试重新连接
                }
                return false;
            }
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="message">要发送的消息</param>
        public async Task SendAsync(string method, object? message)
        {
            string strMessage = JsonConvert.SerializeObject(message);
            Log?.Debug($"发送方法:{method} 信息: {strMessage} 路径:{_hubUrl}");

            // 将消息写入队列，非阻塞
            // _sendQueue.Writer.TryWrite(new MessageRequest(method, strMessage));
            await _sendQueue.Writer.WriteAsync(new MessageRequest(method, strMessage));
        }

        /// <summary>
        /// 同步发送消息（阻塞调用线程）
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="message">要发送的消息</param>
        public void Send(string method, object? message)
        {
            SendAsync(method, message).Wait(); // 阻塞当前线程直到异步操作完成
        }

        /// <summary>
        /// 后台任务：从队列中读取消息并发送
        /// </summary>
        private async Task StartSendingBackgroundTask(CancellationToken cancellationToken)
        {
            const int defaultDelay = 100; // 默认延迟时间（毫秒）
            const int errorDelay = 1000;  // 异常情况下的延迟时间（毫秒）

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // 从队列中读取消息
                    if (await _sendQueue.Reader.WaitToReadAsync(cancellationToken))
                    {
                        while (_sendQueue.Reader.TryRead(out var request))
                        {
                            try
                            {
                                if (_connection != null)
                                {
                                    // 确保连接已建立
                                    if (_connection.State != HubConnectionState.Connected)
                                    {
                                        await ConnectWithRetryAsync(1);
                                    }

                                    // 发送消息
                                    await _connection.SendAsync(request.Method, request.Data);
                                }
                            }
                            catch (Exception ex) when (IsTransientError(ex))
                            {
                                // 如果发送失败，将消息加入待发送缓存
                                _pendingMessages.Enqueue(request);
                                Log?.Warn($"发送失败，消息已加入缓存: {ex.Message}");

                                // 在异常情况下增加延迟
                                await Task.Delay(errorDelay, cancellationToken);
                            }
                        }
                    }
                    else
                    {
                        // 如果队列为空，增加默认延迟
                        await Task.Delay(defaultDelay, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 任务被取消，退出循环
                    break;
                }
                catch (Exception ex)
                {
                    Log?.Error($"后台任务发生异常: {ex}");
                    // 防止任务因未处理异常而退出，增加延迟
                    await Task.Delay(errorDelay, cancellationToken);
                }
            }
        }

        /// <summary>
        /// 处理正在重新连接的状态
        /// </summary>
        private Task OnReconnecting(Exception? exception)
        {
            Log?.Info("连接丢失，正在尝试重新连接...");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 处理已重新连接的状态
        /// </summary>
        private Task OnReconnected(string? connectionId)
        {
            Log?.Info($"已重新连接，连接ID：{connectionId}");
            _ = ResendPendingMessagesAsync(); // 重连成功后重发缓存消息
            return Task.CompletedTask;
        }

        /// <summary>
        /// 处理连接关闭的状态
        /// </summary>
        private async Task OnClosed(Exception? exception)
        {
            Log?.Info("连接已关闭");
            await Task.Delay(_reconnectInterval);
            await ConnectWithRetryAsync(3); // 尝试重新连接
        }

        /// <summary>
        /// 重发缓存中的消息
        /// </summary>
        private async Task ResendPendingMessagesAsync()
        {
            while (_pendingMessages.TryDequeue(out var request))
            {
                try
                {
                    if (_connection != null)
                    {
                        // 假设连接已经成功，直接发送消息
                        await _connection.SendAsync(request.Method, request.Data);
                    }
                }
                catch (Exception ex) when (IsTransientError(ex))
                {
                    // 如果重发失败，重新加入队列
                    _pendingMessages.Enqueue(request);
                    break; // 避免无限重试
                }
            }
        }

        /// <summary>
        /// 判断是否为瞬态错误（可重试）
        /// </summary>
        private bool IsTransientError(Exception ex) =>
            ex is InvalidOperationException || ex is HttpRequestException || ex is TimeoutException;

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _cts.Cancel();
            Stop();
            _connection?.DisposeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 消息请求封装
        /// </summary>
        private record MessageRequest(string Method, string Data);

        /// <summary>
        /// 自定义重连策略类
        /// </summary>
        private class CustomRetryPolicy : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
            {
                return TimeSpan.FromSeconds(1); // 每次重试等待1秒
            }
        }
    }
}
