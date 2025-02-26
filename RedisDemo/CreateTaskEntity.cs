namespace RedisDemo
{
    public class CreateTaskEntity 
    {
        public CreateTaskEntity() { }

        public string? CurrentMethod { get; set; }
        public string? CurrentPoint { get; set; }
        public string? Remark5 { get; set; }
        public string? Remark4 { get; set; }
        public string? Remark3 { get; set; }
        public string? Remark2 { get; set; }
        public string? Remark1 { get; set; }
        public string? WorkUnit { get; set; }
        public int? Line { get; set; }
        public List<string>? Details { get; set; }
        public string? AgvNo { get; set; }
        public string? TaskType { get; set; }
        public string? ToPoint { get; set; }
        public string? FromPoint { get; set; }
        public string? JobId { get; set; }
        public string? IP { get; set; }
        public int? Port { get; set; }
    }
}
