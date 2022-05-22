namespace Storage.Module.Entities.Base
{
    public class Data
    {
        public long UserId { get; set; }
        public decimal AgentEarnUsdt { get; set; }
        public bool IsPaid { get; set; } = false;
        public UserInfo User { get; set; }
    }
}
