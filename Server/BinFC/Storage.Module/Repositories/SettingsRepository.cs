using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<SettingsRepository> _logger;
        public SettingsRepository(DataContext dataContext, ILogger<SettingsRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task<string> SaveChangesAsync()
        {
            try
            {
                await _dataContext.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Join("; ", _dataContext.ChangeTracker.Entries().Select(x => x.Entity.GetType().Name)));
                _dataContext.ChangeTracker.Clear();
                return ex.Message;
            }
        }

        public async Task<(bool, T)> GetSettingsByKeyAsync<T>(string key, bool isNeedTracking = true)
        {
            Settings settingsByKey;

            if (isNeedTracking)
            {
                settingsByKey = await _dataContext.Settings
                   .Where(x => x.Key == key)
                   .FirstOrDefaultAsync();
            }
            else
            {
                settingsByKey = await _dataContext.Settings
                   .AsNoTracking()
                   .Where(x => x.Key == key)
                   .FirstOrDefaultAsync();
            }

            if (settingsByKey != null)
            {
                T settingsValue = (T)Convert.ChangeType(settingsByKey.Value, typeof(T));
                return (true, settingsValue);
            }

            return (false, default(T));
        }

        public async Task<bool> SetSettingsByKeyAsync(string key, object value)
        {
            Settings settingsByKey;

            settingsByKey = await _dataContext.Settings
               .Where(x => x.Key == key)
               .FirstOrDefaultAsync();

            if (settingsByKey != null)
            {
                settingsByKey.Value = value.ToString();
                _dataContext.Add(settingsByKey);

                await SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}