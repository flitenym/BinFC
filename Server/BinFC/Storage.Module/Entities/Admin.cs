using System.ComponentModel.DataAnnotations;

namespace Storage.Module.Entities
{
    public class Admin
    {
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
    }
}