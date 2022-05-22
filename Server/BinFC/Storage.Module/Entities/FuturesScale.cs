using Storage.Module.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Storage.Module.Entities
{
    public class FuturesScale : Scale
    {
        [Key]
        public long Id { get; set; }
    }
}