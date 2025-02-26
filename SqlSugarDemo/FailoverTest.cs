using SqlSugar;

namespace SqlSugarDemo
{
    // 定义表的模型类
    [SugarTable("FailoverTest")]
    public class FailoverTest
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] // 主键和自增列
        public long Id { get; set; }

        [SugarColumn(Length = 255, IsNullable = false)]
        public string Data { get; set; } = string.Empty;

        [SugarColumn(IsNullable = false)]
        public DateTime CreateTime { get; set; }
    }
}
