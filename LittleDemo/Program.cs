namespace LittleDemo
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Text.RegularExpressions;

    class A
    {
        public A()
        {
            PrintFields(); // 在构造函数中调用 PrintFields
        }

        public virtual void PrintFields() { }
    }

    class B : A
    {
        int x = 1; // x 初始化为 1
        int y; // y 默认值为 0

        public B()
        {
            y = -1; // 在 B 的构造函数中将 y 赋值为 -1
        }

        public override void PrintFields()
        {
            Console.WriteLine("x={0}, y={1}", x, y); // 打印 x 和 y 的值
        }
    }

    //class Program
    //{
    //    static void Main()
    //    {
    //        #region
    //        //// 强制使用 TLS 1.2
    //        ////ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    //        //// SMTP服务器地址和端口号
    //        //string smtpServer = "smtp.qq.com";
    //        //int smtpPort = 25; // 或者465（SSL）

    //        //// 发件人邮箱地址和授权码
    //        //string senderEmail = "1192832939@qq.com";
    //        //string senderPassword = "uqyqasokippqhegf";
    //        ////string senderEmail = "kael_tian@163.com";
    //        ////string senderPassword = "UKTJOHTTHXNAONTU";

    //        //// 收件人邮箱地址
    //        //string recipientEmail = "daiyin_zhang@163.com";

    //        //// 创建邮件内容
    //        //MailMessage mail = new MailMessage();
    //        //mail.From = new MailAddress(senderEmail);
    //        //mail.To.Add(recipientEmail);
    //        //mail.Subject = "邮件主题12";
    //        //mail.Body = "这是邮件正文12";

    //        //// 配置SMTP客户端
    //        //SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
    //        //{
    //        //    Credentials = new NetworkCredential(senderEmail, senderPassword),
    //        //    EnableSsl = false, // 使用SSL加密
    //        //    Timeout = 20000, // 设置超时时间为20秒
    //        //    DeliveryMethod = SmtpDeliveryMethod.Network
    //        //};
    //        //try
    //        //{
    //        //    smtpClient.Send(mail);
    //        //    Console.WriteLine("邮件发送成功");
    //        //}
    //        //catch (SmtpException smtpEx)
    //        //{
    //        //    Console.WriteLine("SMTP错误: " + smtpEx.Message);
    //        //}
    //        //catch (IOException ioEx)
    //        //{
    //        //    Console.WriteLine("IO错误: " + ioEx.Message);
    //        //}
    //        //catch (Exception ex)
    //        //{
    //        //    Console.WriteLine("其他错误: " + ex.Message);
    //        //}
    //        #endregion

    //        //new B(); // 创建 B 的实例

    //        //String s = "";

    //        string input = "   This    is   a   test   string   ";

    //        string trimmedInput=input.Trim();

    //        string result = Regex.Replace(trimmedInput, @"\s+", " ");

    //        Console.WriteLine(result);


    //        Console.ReadLine();
    //    }
    //}


    /// <summary>
    /// 测试并发信号量
    /// </summary>
    class Program
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(2); // 最大并发线程数设置为 2

        static async Task AccessSharedResourceAsync(int taskId)
        {
            Console.WriteLine($"Task {taskId} is waiting to enter...");

            // 尝试获取信号量（等待进入临界区）
            await semaphore.WaitAsync();

            try
            {
                // 模拟访问共享资源
                Console.WriteLine($"Task {taskId} has entered,start time:{DateTime.Now}.");
                await Task.Delay(2000); // 模拟工作
                Console.WriteLine($"Task {taskId} is leaving,end time:{DateTime.Now}.");
            }
            finally
            {
                // 释放信号量
                semaphore.Release();
            }
        }

        static void Main(string[] args)
        {
            //// 创建多个任务以演示并发
            //Task[] tasks = new Task[5];

            //for (int i = 0; i < 5; i++)
            //{
            //    int taskId = i + 1;
            //    tasks[i] = AccessSharedResourceAsync(taskId);
            //}

            //// 等待所有任务完成
            //await Task.WhenAll(tasks);

            //Console.WriteLine("All tasks have completed.");

            //var localTime=DateTimeOffset.Now; // 本地时间
            //var utcTime=DateTimeOffset.UtcNow; // UTC时间
            //var offset=localTime.Offset; // 偏移量


            //Console.WriteLine($"本地时间: {localTime}");   // 2024-11-22 10:00:00 +08:00
            //Console.WriteLine($"UTC 时间: {utcTime}");    // 2024-11-22 02:00:00 +00:00
            //Console.WriteLine($"偏移量: {offset}");       // +08:00

            //var ticks1=DateTimeOffset.UtcNow.Ticks;
            //var ticks2=DateTime.UtcNow.Ticks;

            //// 从 DateTime.Now 获取当前时间的 DateTimeOffset
            //var dateTime = DateTime.Now;
            //var dateTimeOffset = new DateTimeOffset(dateTime);
            //Console.WriteLine(dateTimeOffset);  // 输出类似：2024-11-22 10:00:00 +08:00

            //// 获取当前系统的时区偏移量
            //var offset1 = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            //Console.WriteLine(offset);  // 输出：08:00:00

            //Console.ReadLine();

            Publisher publisher = new Publisher();

            // 外部覆盖委托，原有逻辑丢失
            publisher.OnNotify = message => Console.WriteLine($"Bad subscriber: {message}");

            // 添加正常订阅
            publisher.OnNotify = message => Console.WriteLine($"Normal subscriber: {message}");

            // 调用委托
            publisher.OnNotify?.Invoke("Oops!");
        }

        public delegate void Notify(string message);

        class Publisher
        {
            public Notify? OnNotify; // 直接暴露委托（不安全）
        }


    }


}
