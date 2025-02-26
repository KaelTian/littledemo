using SqlSugar;

namespace SqlSugarDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await FailoverPerformanceTestAsync();
            Console.ReadLine();
        }


        public static async Task FailoverPerformanceTestAsync()
        {
            // 数据库连接配置
            var connectionString = "Server=192.168.60.209;Port=13306;Database=linton;User=root;Password=admin;Connection Timeout=30;";

            // 初始化 SqlSugarClient
            var db = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = DbType.MySql, // 使用 MySQL
                IsAutoCloseConnection = true, // 自动关闭连接
                InitKeyType = InitKeyType.Attribute // 从模型映射
            });

            // 确保表存在
            db.CodeFirst.InitTables<FailoverTest>();

            Console.WriteLine("开始持续写入数据到 MySQL 数据库...");
            var random = new Random();

            while (true)
            {
                try
                {
                    // 准备要插入的数据
                    int rowsToInsert = random.Next(1, 6); // 1 到 5 条随机数量
                    var records = new List<FailoverTest>();

                    for (int i = 0; i < rowsToInsert; i++)
                    {
                        records.Add(new FailoverTest
                        {
                            Data = $"SampleData-{Guid.NewGuid()}",
                            CreateTime = DateTime.Now
                        });
                    }

                    // 批量插入数据
                    await db.Insertable(records).ExecuteCommandAsync();

                    Console.WriteLine($"成功写入 {rowsToInsert} 条数据 - {DateTime.Now}");

                    // 暂停一段时间（例如，3秒）
                    await Task.Delay(3000);
                }
                catch (Exception ex)
                {
                    // 捕获异常并输出
                    Console.WriteLine($"写入数据时出错：{ex.Message}");
                    await Task.Delay(3000);
                }
            }
        }
    }
}
