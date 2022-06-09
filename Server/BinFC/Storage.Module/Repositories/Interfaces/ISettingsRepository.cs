using Storage.Module.Classes;
using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        public IEnumerable<Settings> Get();
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
        public Task<(bool IsSuccess, T Value)> GetSettingsByKeyAsync<T>(string key, bool isNeedTracking = true);
        public Task<string> SetSettingsByKeyAsync(string key, object value);
        public Task<SettingsInfo> GetSettingsAsync();
    }
}
