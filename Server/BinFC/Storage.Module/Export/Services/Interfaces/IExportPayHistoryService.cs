using System.Threading.Tasks;

namespace Storage.Module.Export.Services.Interfaces
{
    public interface IExportPayHistoryService
    {
        public Task<(bool IsSuccess, string Error, byte[] FileContent)> ExportAsync(long[] ids);
    }
}