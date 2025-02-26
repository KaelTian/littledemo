using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 12000;
UdpClient udpServer = new UdpClient(port);

Console.WriteLine("UDP 服务器已启动,等待客户端消息...");

while (true)
{
    // 接收客户端消息
    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, port);
    UdpReceiveResult result = await udpServer.ReceiveAsync();
    string data = Encoding.UTF8.GetString(result.Buffer);
    clientEndPoint = result.RemoteEndPoint;
    Console.WriteLine($"收到来自 {clientEndPoint} 的消息: {data}");

    // 向客户端发送响应
    string response = $"已收到消息: {data}";
    byte[] msg = Encoding.UTF8.GetBytes(response);
    await udpServer.SendAsync(msg, msg.Length, clientEndPoint);
    Console.WriteLine($"已向 {clientEndPoint} 发送响应: {response}");
}