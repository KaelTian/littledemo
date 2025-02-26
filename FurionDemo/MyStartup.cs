using Furion;

namespace FurionDemo
{
    public class MyStartup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("调用田卡的MyStartup服务啦");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine("调用田卡中间价注册啦");
        }
    }
}
