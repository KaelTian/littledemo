using Newtonsoft.Json;
using RedisDemo;
using StackExchange.Redis;

try
{
    // 连接到 Redis 服务器
    // 这里假设 Redis 服务器运行在本地，端口为 6379
    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("192.168.60.152:6379,password=1qaz2wsxE");

    // 获取数据库实例，默认数据库编号为 0
    IDatabase db = redis.GetDatabase();

    //// 设置一个键值对
    //string key = "myKey";
    //string value = "Hello, Redis!";
    //db.StringSet(key, value);

    //// 获取键对应的值
    //string retrievedValue = db.StringGet(key);
    //Console.WriteLine($"Retrieved value for key '{key}': {retrievedValue}");

    //// 删除键
    //bool isDeleted = db.KeyDelete(key);
    //if (isDeleted)
    //{
    //    Console.WriteLine($"Key '{key}' has been deleted.");
    //}
    //else
    //{
    //    Console.WriteLine($"Failed to delete key '{key}'.");
    //}
    string redisKey = "LQ_RefeedBucketCache_FurnaceArea_Task1";
    CreateTaskEntity taskEntity = new CreateTaskEntity()
    {
        AgvNo = "AgvNo1",
        CurrentMethod = "Method1",
        Details = new List<string>
            {
                "1",
                "2",
                "3"
            }
    };
    var count = ZAdd(db, redisKey, (CommonUtility.TimeScore - 1, taskEntity), (CommonUtility.TimeScore, taskEntity), (CommonUtility.TimeScore + 1, taskEntity));
    (CreateTaskEntity? member, decimal score)[] values = ZGetMin<CreateTaskEntity>(db, redisKey, 5);
    (CreateTaskEntity? member, decimal score)[] values1 = ZPopMin<CreateTaskEntity>(db, redisKey, 5);
    // 关闭连接
    redis.Close();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

static long ZAdd(IDatabase db, string key, params (decimal score, object member)[] scoreMembers)
{
    if (string.IsNullOrEmpty(key))
        throw new ArgumentException("Key cannot be null or empty.", nameof(key));

    if (scoreMembers == null || scoreMembers.Length == 0)
        throw new ArgumentException("Score members cannot be null or empty.", nameof(scoreMembers));

    // 将 (score, member) 对转换为 SortedSetEntry 数组
    var entries = scoreMembers.Select(sm => new SortedSetEntry(
        JsonConvert.SerializeObject(sm.member), // 将对象序列化为 JSON 字符串
        (double)sm.score                        // 将 decimal 转换为 double
    )).ToArray();

    // 使用 SortedSetAdd 方法将数据添加到有序集合中
    return db.SortedSetAdd(key, entries);
}

static (T? member, decimal score)[] ZPopMin<T>(IDatabase db, string key, long count)
{
    if (string.IsNullOrEmpty(key))
        throw new ArgumentException("Key cannot be null or empty.", nameof(key));

    if (count <= 0)
        throw new ArgumentException("Count must be greater than 0.", nameof(count));

    // 获取并移除最小分数的元素
    var entries = db.SortedSetRangeByScoreWithScores(key, order: Order.Ascending, take: count);

    if (entries.Length == 0)
        return Array.Empty<(T?, decimal)>(); // 如果没有元素，返回空数组

    var result = new List<(T?, decimal)>();

    foreach (var entry in entries)
    {
        // 将 JSON 字符串反序列化为对象
        T? member = JsonConvert.DeserializeObject<T>(entry.Element.ToString());

        // 将分数从 double 转换为 decimal
        decimal score = (decimal)entry.Score;

        result.Add((member, score));

        // 从有序集合中移除该元素
        db.SortedSetRemove(key, entry.Element);
    }
    return result.ToArray();
}

static (T? member, decimal score)[] ZGetMin<T>(IDatabase db, string key, long count)
{
    if (string.IsNullOrEmpty(key))
        throw new ArgumentException("Key cannot be null or empty.", nameof(key));

    if (count <= 0)
        throw new ArgumentException("Count must be greater than 0.", nameof(count));

    // 获取最小分数的元素
    var entries = db.SortedSetRangeByScoreWithScores(key, order: Order.Ascending, take: count);

    if (entries.Length == 0)
        return Array.Empty<(T?, decimal)>(); // 如果没有元素，返回空数组

    var result = new List<(T?, decimal)>();

    foreach (var entry in entries)
    {
        // 将 JSON 字符串反序列化为对象
        T? member = JsonConvert.DeserializeObject<T>(entry.Element.ToString());

        // 将分数从 double 转换为 decimal
        decimal score = (decimal)entry.Score;

        result.Add((member, score));
    }
    return result.ToArray();
}