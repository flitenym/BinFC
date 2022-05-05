using Storage.Module.Entities;
using Storage.Module.Services.Interfaces;
using Storage.Module.StaticClasses;
using System.Threading.Tasks;

namespace Storage.Module.Services
{
    public class InitialCreateService : IInitialCreateService
    {
        private readonly DataContext _dataContext;
        public InitialCreateService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task InitialCreateValuesAsync()
        {
            if (_dataContext.Database.EnsureCreated())
            {
                foreach (string settingsKey in SettingsKeys.Settings)
                {
                    Settings settings = new Settings()
                    {
                        Key = settingsKey
                    };

                    _dataContext.Add(settings);
                }

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}