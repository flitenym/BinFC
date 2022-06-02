using System.ComponentModel.DataAnnotations;

namespace Storage.Module.Entities
{
    public class TelegramMessageQueue
    {
        [Key]
        public long Id { get; set; }
        public long? ChatId { get; set; }
        public string Message { get; set; }
    }
}