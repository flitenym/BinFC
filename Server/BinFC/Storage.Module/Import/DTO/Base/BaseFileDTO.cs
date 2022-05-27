using Microsoft.AspNetCore.Http;
using System.IO;

namespace Storage.Module.Import.DTO.Base
{
    public abstract class BaseFileDTO : BaseDTO
    {
        public byte[] GetFileContent(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}