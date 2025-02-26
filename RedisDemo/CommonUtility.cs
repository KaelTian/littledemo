namespace RedisDemo
{
    public static class CommonUtility
    {
        public static decimal TimeScore => decimal.Parse(DateTime.Now.ToString("yyyyMMddhhhmmss"));
    }
}
