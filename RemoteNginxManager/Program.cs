using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;

static void FileTransfer(WSManConnectionInfo connectionInfo, string localPath, string remoteServer, string remotePath)
{
    string? remoteShare = Path.GetDirectoryName(remotePath)?.Replace("C:", $@"\\{remoteServer}\C$");

    if (string.IsNullOrEmpty(remoteShare))
        throw new ArgumentException("Invalid remote path.");

    // 验证本地文件是否存在
    if (!File.Exists(localPath))
        throw new FileNotFoundException($"Local file '{localPath}' does not exist.");
    // 创建远程运行空间
    using (var runspace = RunspaceFactory.CreateRunspace(connectionInfo))
    {
        runspace.Open();
        // 创建远程共享路径的临时目录（如果不存在）
        using (var ps = PowerShell.Create())
        {
            ps.Runspace = runspace;
            // Step 1: 创建目标目录（如果不存在）
            string? remoteDirectory = Path.GetDirectoryName(remotePath);
            ps.AddScript($@"
                    if (-not (Test-Path '{remoteDirectory}')) {{
                        New-Item -ItemType Directory -Path '{remoteDirectory}' -Force;
                    }}
                ");
            ps.Invoke();

            if (ps.HadErrors)
            {
                throw new Exception("Failed to prepare remote directory. Ensure credentials and paths are correct.");
            }
        }
    }
    // 使用 File.Copy 复制文件
    string remoteFilePath = Path.Combine(remoteShare, Path.GetFileName(remotePath));
    File.Copy(localPath, remoteFilePath, overwrite: true);

    Console.WriteLine($"File successfully uploaded to: {remoteFilePath}");
}
static void SyncNginxFile(WSManConnectionInfo connectionInfo, string remoteConfigPath, string remoteTempPath, string nginxPath)
{
    using (var runspace = RunspaceFactory.CreateRunspace(connectionInfo))
    {
        runspace.Open();
        using (var ps = PowerShell.Create())
        {
            ps.Runspace = runspace;

            // 备份原配置文件
            ps.AddScript($"Copy-Item -Path \"{remoteConfigPath}\" -Destination \"{remoteConfigPath}.backup\" -Force");
            ps.Invoke();

            // 替换配置文件（使用已上传的临时文件）
            ps.Commands.Clear();
            ps.AddScript($"Copy-Item -Path \"{remoteTempPath}\" -Destination \"{remoteConfigPath}\" -Force");
            ps.Invoke();

            // 验证 Nginx 配置
            ps.Commands.Clear();
            ps.AddScript($@"
                        cd ""{nginxPath}"";
                        ./nginx.exe -t; 
                        ");
            ps.Invoke();
            List<string> testOutputMessages = new List<string>();
            if (ps.HadErrors)
            {
                // 输出错误信息
                foreach (var error in ps.Streams.Error)
                {
                    Console.WriteLine($"Error: {error}");
                    testOutputMessages.Add(error.ToString());
                }
            }
            else
            {
                // 输出标准输出
                foreach (var information in ps.Streams.Information)
                {
                    Console.WriteLine($"Information: {information}");
                    testOutputMessages.Add(information.ToString());
                }
            }
            if (testOutputMessages.Count > 0 &&
                testOutputMessages.Any(a => a.Contains("syntax is ok")) &&
                testOutputMessages.Any(a => a.Contains("test is successful")))
            {
                // 重载 Nginx
                ps.Commands.Clear();
                ps.AddScript($@"
                        cd ""{nginxPath}"";
                        ./nginx.exe -s reload;
                        ");
                //ps.AddScript($"cd \"{nginxPath}\"; ./nginx.exe -s reload");
                var newResult = ps.Invoke();
                if (ps.HadErrors)
                {
                    // 输出错误信息
                    foreach (var error in ps.Streams.Error)
                    {
                        Console.WriteLine($"Reload error: {error}");
                    }
                }
                else
                {
                    Console.WriteLine("Nginx reloaded successfully.");
                }
            }
            else
            {
                Console.WriteLine("Configuration test failed. Rolling back...");
                ps.Commands.Clear();
                ps.AddScript($"Copy-Item -Path \"{remoteConfigPath}.backup\" -Destination \"{remoteConfigPath}\" -Force");
                ps.Invoke();
            }
        }
    }
}
#region 192.168.60网段配置
//string remoteServer = "192.168.60.209";
//string username = "田赛";
//string password = "ts6yhn7ujm^&*I";
//string remoteConfigPath = @"D:\Softwares\nginx-1.26.0\nginx-1.26.0\conf\nginx.conf";
//string localConfigPath = @"D:\works\双良\nginx-demo-folder\nginx-60-209-main.conf";
//string remoteTempPath = @"C:\Temp\nginx-replace.conf"; // 临时文件路径
//string nginxPath = @"D:\Softwares\nginx-1.26.0\nginx-1.26.0";
#endregion
#region 192.168.0网段配置
string remoteServer = "192.168.0.101";
string username = "Administrator";
string password = "123456";
string remoteConfigPath = @"D:\nginx-1.26.2\conf\nginx.conf";
string localConfigPath = @"D:\works\双良\nginx-demo-folder\nginx-0-120-main.conf";
string remoteTempPath = @"C:\Temp\nginx-replace.conf"; // 临时文件路径
string nginxPath = @"D:\nginx-1.26.2";
#endregion
Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
try
{
    Console.WriteLine($"Begin to sync nginx config settings,current time:{DateTime.Now}.");
    var connectionInfo = new WSManConnectionInfo(
        new Uri($"http://{remoteServer}:5985/wsman"),
        "http://schemas.microsoft.com/powershell/Microsoft.PowerShell",
        new PSCredential(username, ConvertToSecureString(password))
    );
    // Step 1: 将本地文件上传到远程服务器
    FileTransfer(connectionInfo, localConfigPath, remoteServer, remoteTempPath);
    // Step 2: 在远程服务器上执行 PowerShell 命令
    SyncNginxFile(connectionInfo, remoteConfigPath, remoteTempPath, nginxPath);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    stopwatch.Stop();
    // 获取总秒数并保留三位小数
    double totalSeconds = stopwatch.Elapsed.TotalSeconds;
    string formattedTime = totalSeconds.ToString("F3");
    Console.WriteLine($"End to sync nginx config settings,current time:{DateTime.Now} execute times:{formattedTime}(s).");
}
Console.WriteLine("Press any key to exit.");
Console.ReadKey();

static SecureString ConvertToSecureString(string input)
{
    if (string.IsNullOrEmpty(input))
        throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty.");

    SecureString secureString = new SecureString();
    foreach (char c in input)
    {
        secureString.AppendChar(c);
    }

    secureString.MakeReadOnly(); // 可选：将 SecureString 标记为只读以增强安全性
    return secureString;
}