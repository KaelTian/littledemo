using System.Net;
using System.Net.Sockets;
using System.Text;
const int Port = 12345;


var listener = new TcpListener(IPAddress.Any, Port);
listener.Start();
Console.WriteLine("Server started...");

while (true)
{
    var client = listener.AcceptTcpClient();
    Console.WriteLine("Client connected...");

    var stream = client.GetStream();

    // 读取客户端发送的消息长度（前4个字节）
    byte[] lengthBuffer = new byte[4];
    stream.Read(lengthBuffer, 0, 4);
    int dataLength = BitConverter.ToInt32(lengthBuffer, 0);
    Console.WriteLine($"Received data length: {dataLength}");

    // 读取客户端发送的数据内容
    byte[] dataBuffer = new byte[dataLength];
    stream.Read(dataBuffer, 0, dataLength);
    string receivedMessage = Encoding.UTF8.GetString(dataBuffer);
    Console.WriteLine($"Received message: {receivedMessage}");

    // 假设根据接收到的数据进行某些处理（这里我们简单地回复一个固定消息）
    string responseMessage = "Message processed successfully!";
    byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
    byte[] responseLength = BitConverter.GetBytes(responseData.Length);

    // 发送响应数据：先发送长度，再发送数据
    stream.Write(responseLength, 0, responseLength.Length);
    stream.Write(responseData, 0, responseData.Length);
    Console.WriteLine($"Sent response to client: {responseMessage}");

    client.Close();
}