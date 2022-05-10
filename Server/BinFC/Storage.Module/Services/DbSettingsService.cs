using Storage.Module.Services.Interfaces;

namespace Storage.Module.Services
{
    public class DbSettingsService : IDbSettingsService
    {
        public DbSettings GetDbSettings()
        {
            return DbSettings.GetInstance();
        }
    }
}
