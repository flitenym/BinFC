using Storage.Module.Entities;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        public Task DataContextSaveChanges();
        public Task<(bool, T)> GetSettingsByKey<T>(string key, bool isNeedTracking = true);
        public Task<bool> SetSettingsByKey(string key, object value);
    }
}
