using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Module.Entities.Base
{
    public class Scale
    {
        public long FromValue { get; set; }
        public long Percent { get; set; }
        public long UniqueId { get; set; }

        [ForeignKey("UniqueId")]
        public virtual Unique Unique { get; set; }
    }
}
