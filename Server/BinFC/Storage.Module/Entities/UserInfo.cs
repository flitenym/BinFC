namespace Storage.Module.Entities
{
    public class UserInfo
    {
        public long Id { get; set; }
        public long ChatId { get; set; }
        /// <summary>
        /// Реферальный ИД (указывается Администратором)
        /// </summary>
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string TrcAddress { get; set; }
        public string BepAddress { get; set; }
        public bool UniqueString { get; set; }
    }
}
