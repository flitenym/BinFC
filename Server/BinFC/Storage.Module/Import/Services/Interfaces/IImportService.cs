using Storage.Module.Import.Enums;
using System.Threading.Tasks;

namespace Storage.Module.Import.Services.Interfaces
{
    public interface IImportService
    {
        public Task<(bool IsSuccess, string Error)> ImportAsync(byte[] fileContent, ImportType importType);
    }
}