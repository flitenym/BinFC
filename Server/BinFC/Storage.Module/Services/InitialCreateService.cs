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
                // базовые настройки
                foreach (string settingsKey in SettingsKeys.Settings)
                {
                    Settings settings = new Settings()
                    {
                        Key = settingsKey
                    };

                    _dataContext.Add(settings);
                }

                // admin
                Admin admin = new Admin()
                {
                    UserName = "admin",
                    Password = "Qqwerty1234!"
                };

                _dataContext.Add(admin);

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}