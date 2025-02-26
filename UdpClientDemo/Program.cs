using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 12000;
UdpClient udpClient = new UdpClient();

Console.WriteLine("UDP 客户端已启动，请输入消息（输入 Q 退出）：");

while (true)
{
    // 从控制台读取输入
    string? input = Console.ReadLine();
    if (input == null || string.IsNullOrWhiteSpace(input))
    {
        continue;
    }
    if (input?.ToUpper() == "Q")
    {
        Console.WriteLine("客户端已退出");
        break;
    }
    // 发送消息到服务器
    byte[] msg = Encoding.UTF8.GetBytes(input ?? string.Empty);
    await udpClient.SendAsync(msg, msg.Length, "192.168.0.210", port);

    // 接收服务器响应
    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, port);
    UdpReceiveResult result = await udpClient.ReceiveAsync();
    string response = Encoding.UTF8.GetString(result.Buffer);
    Console.WriteLine($"收到服务器响应: {response}");
}

udpClient.Close();