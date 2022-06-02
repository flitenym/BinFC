using System.ComponentModel.DataAnnotations;

namespace Storage.Module.Entities
{
    public class Unique
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}