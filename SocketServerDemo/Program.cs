using System.Net;
using System.Net.Sockets;
using System.Text;

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 11000;

// 创建TCP/IP套接字
Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
try
{
    // 绑定套接字到本地端点并监听连接
    listener.Bind(new IPEndPoint(ipAddress, port));
    listener.Listen(10);

    Console.WriteLine("等待客户端连接...");

    // 接受客户端连接
    Socket handler =await listener.AcceptAsync();

    // 接收客户端发送的数据
    byte[] buffer = new byte[1024];
    int totalBytesReceived = 0;
    StringBuilder receivedData = new StringBuilder();

    while (true)
    {
        int bytesReceived = await handler.ReceiveAsync(buffer);
        if (bytesReceived == 0)
        {
            // 客户端关闭连接
            Console.WriteLine("客户端关闭连接");
            break;
        }

        // 将接收到的数据追加到StringBuilder
        receivedData.Append(Encoding.UTF8.GetString(buffer, 0, bytesReceived));
        totalBytesReceived += bytesReceived;
    }

    Console.WriteLine("收到客户端消息: " + receivedData.ToString());

    // 向客户端发送响应
    string response = "消息已收到，总字节数: " + totalBytesReceived;
    byte[] msg = Encoding.UTF8.GetBytes(response);
    await handler.SendAsync(msg);

    // 关闭连接
    handler.Shutdown(SocketShutdown.Both);
    handler.Close();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    listener.Close();
}