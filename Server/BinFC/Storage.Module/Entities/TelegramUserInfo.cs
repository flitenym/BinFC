using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Storage.Module.Entities
{
    public class TelegramUserInfo
    {
        [Key]
        public long Id { get; set; }
        public long? ChatId { get; set; }
        public string LastCommand { get; set; }
        public string Language { get; set; } = DefaultValues.Languages.First();
    }
}