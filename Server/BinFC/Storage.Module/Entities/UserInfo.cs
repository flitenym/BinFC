using Storage.Module.Entities.Base;

namespace Storage.Module.Entities
{
    public class UserInfo : BaseEntity
    {
        public long ChatId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public bool UniqueString { get; set; }
    }
}
