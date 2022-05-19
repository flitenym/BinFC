using Storage.Module.Import.Enums;
using Storage.Module.Import.Services.Interfaces;
using System.Threading.Tasks;

namespace Storage.Module.Import.Services
{
    public class ImportService : IImportService
    {
        public async Task<(bool IsSuccess, string Error)> ImportAsync(byte[] fileContent, ImportType importType)
        {
            

            return default;
        }
    }
}
