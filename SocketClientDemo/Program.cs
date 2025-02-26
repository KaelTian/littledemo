// 设置服务器IP和端口
using System.Net.Sockets;
using System.Net;
using System.Text;

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 11000;

// 创建TCP/IP套接字
Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

try
{
    // 连接到服务器
    await sender.ConnectAsync(new IPEndPoint(ipAddress, port));
    Console.WriteLine("已连接到服务器");

    // 发送数据到服务器
    string message = "你好，服务器！这是一个较长的消息，用于测试大数据量的接收。";
    byte[] msg = Encoding.UTF8.GetBytes(message);
    await sender.SendAsync(msg, SocketFlags.None);

    // 关闭发送通道，表示数据发送完毕
    sender.Shutdown(SocketShutdown.Send);
    Console.WriteLine("数据发送完毕，等待服务器响应...");

    // 接收服务器的响应
    byte[] buffer = new byte[1024];
    int bytesReceived = await sender.ReceiveAsync(buffer, SocketFlags.None);
    string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
    Console.WriteLine("收到服务器响应: " + response);

    // 关闭连接
    sender.Shutdown(SocketShutdown.Both);
    sender.Close();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}