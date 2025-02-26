using System.Net.Sockets;
using System.Text;

const string ServerAddress = "127.0.0.1";
const int Port = 12345;

var client = new TcpClient(ServerAddress, Port);
var stream = client.GetStream();

// 构建要发送的数据
string requestMessage = "Hello from client!";
byte[] data = Encoding.UTF8.GetBytes(requestMessage);
byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

// 发送请求数据：前4个字节表示数据长度，接着是数据内容
stream.Write(lengthPrefix, 0, lengthPrefix.Length);
stream.Write(data, 0, data.Length);
Console.WriteLine($"Sent message to server: {requestMessage}");

// 读取服务器响应的消息长度
byte[] lengthBuffer = new byte[4];
stream.Read(lengthBuffer, 0, 4);
int responseLength = BitConverter.ToInt32(lengthBuffer, 0);
Console.WriteLine($"Received response data length: {responseLength}");

// 读取服务器响应的数据内容
byte[] responseData = new byte[responseLength];
stream.Read(responseData, 0, responseLength);
string responseMessage = Encoding.UTF8.GetString(responseData);
Console.WriteLine($"Received response from server: {responseMessage}");

client.Close();