using Storage.Module.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Storage.Module.Entities
{
    public class FuturesData : Data
    {
        [Key]
        public long Id { get; set; }
    }
}