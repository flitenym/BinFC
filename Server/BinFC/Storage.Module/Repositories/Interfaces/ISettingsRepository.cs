using Storage.Module.Entities;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        public Task<string> SaveChangesAsync();
        public Task<(bool IsSuccess, T Value)> GetSettingsByKeyAsync<T>(string key, bool isNeedTracking = true);
        public Task<bool> SetSettingsByKeyAsync(string key, object value);
    }
}
