using System.ComponentModel.DataAnnotations;

namespace Storage.Module.Entities
{
    public class Settings
    {
        [Key]
        public long Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}