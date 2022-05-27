using Storage.Module.Export.DTO.Base;
using System.Linq;

namespace Storage.Module.Export.DTO
{
    public class ExportPayHistoryDTO : BaseDTO
    {
        public long[] Ids { get; set; }
        public override bool IsValid()
        {
            return Ids != null && Ids.Any();
        }
    }
}