using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Classes;
using Storage.Module.Entities;
using Storage.Module.Localization;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<SettingsRepository> _logger;
        public SettingsRepository(DataContext dataContext, IBaseRepository baseRepository, ILogger<SettingsRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _logger = logger;
        }

        public IEnumerable<Settings> Get()
        {
            return _dataContext
                .Settings
                .OrderBy(x => x.Id);
        }

        public Task<(bool IsSuccess, string Message)> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }

        public async Task<(bool IsSuccess, T Value)> GetSettingsByKeyAsync<T>(string key, bool isNeedTracking = true)
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

        public async Task<string> SetSettingsByKeyAsync(string key, object value)
        {
            Settings settingsByKey;

            settingsByKey = await _dataContext.Settings
               .Where(x => x.Key == key)
               .FirstOrDefaultAsync();

            if (settingsByKey != null)
            {
                settingsByKey.Value = value?.ToString();
                _dataContext.Update(settingsByKey);
                return null;
            }

            return string.Format(StorageLoc.NotFoundSettingsByKey, key);
        }

        public async Task<SettingsInfo> GetSettingsAsync()
        {
            List<Settings> settings = await _dataContext.Settings.AsNoTracking().ToListAsync();

            SettingsInfo settingsInfo = new SettingsInfo();

            settingsInfo.SetFieldsBySettings(settings);

            return settingsInfo;
        }
    }
}