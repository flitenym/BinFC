using Microsoft.AspNetCore.Http;
using Storage.Module.Import.DTO.Base;
using Storage.Module.Import.Enums;

namespace Storage.Module.Import.DTO
{
    public class ImportDTO : BaseFileDTO
    {
        public IFormFile File { get; set; }
        public ImportType ImportType { get; set; }

        public override bool IsValid()
        {
            return File != null;
        }
    }
}