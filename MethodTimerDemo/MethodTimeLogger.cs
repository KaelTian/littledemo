using System.Reflection;

namespace MethodTimerDemo
{
    public static class MethodTimeLogger
    {
        public static void Log(MethodBase methodBase, TimeSpan elapsed, string message)
        {
            Console.WriteLine($"方法：{methodBase.Name} 耗时：{elapsed}, 信息：{message}");
        }
    }
}
