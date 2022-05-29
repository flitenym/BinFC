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
                Settings apiKey = new Settings()
                {
                    Key = SettingsKeys.ApiKey,
                    Value = null
                };
                _dataContext.Add(apiKey);

                Settings apiSecret = new Settings()
                {
                    Key = SettingsKeys.ApiSecret,
                    Value = null
                };
                _dataContext.Add(apiSecret);

                Settings cronExpression = new Settings()
                {
                    Key = SettingsKeys.CronExpression,
                    Value = DefaultValues.Cron
                };
                _dataContext.Add(cronExpression);

                Settings sellCurrency = new Settings()
                {
                    Key = SettingsKeys.SellCurrency,
                    Value = DefaultValues.SellCurrency
                };
                _dataContext.Add(sellCurrency);

                Settings isNotification = new Settings()
                {
                    Key = SettingsKeys.IsNotification,
                    Value = bool.TrueString
                };
                _dataContext.Add(isNotification);

                Settings binanceSellEnable = new Settings()
                {
                    Key = SettingsKeys.BinanceSellEnable,
                    Value = bool.FalseString
                };
                _dataContext.Add(binanceSellEnable);

                // admin
                Admin admin = new Admin()
                {
                    UserName = DefaultValues.AdminName,
                    Password = DefaultValues.AdminPassword
                };
                _dataContext.Add(admin);

                // unique
                Unique unique = new Unique()
                {
                    Name = DefaultValues.UniqueName,
                    IsDefault = true
                };
                _dataContext.Add(unique);

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}