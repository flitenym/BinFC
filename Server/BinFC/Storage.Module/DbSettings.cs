using HostLibrary.Enum;
using Microsoft.Extensions.Configuration;
using System;

namespace Storage.Module
{
    public class DbSettings
    {
        private static readonly Lazy<DbSettings> lazy = new(() => new DbSettings());

        public DbProviderType DbProviderType { get; set; }
        public string ConnectionString { get; set; }
        public int RetryAttemts { get; set; }
        
        private DbSettings()
        {
        }

        public static DbSettings GetInstance()
        {
            return lazy.Value;
        }

        public static DbSettings Initialize(IConfiguration configuration)
        {
            if (configuration != null)
            {
                return SetSettingsInfo(GetInstance(), configuration);
            }

            return GetInstance();
        }

        private static DbSettings SetSettingsInfo(DbSettings dbSettings, IConfiguration configuration)
        {
            DbProviderType dbProviderType =
                DbProviderTypeConverter.ConvertStringToDbProvider(configuration.GetSection("Project:DbProvider").Get<string>());

            if (dbProviderType == DbProviderType.Undefined)
            {
                throw new ApplicationException("Project.DbProvider не указан в настройках appsettings.json");
            }

            string connectionString = configuration.GetSection("ConnectionStrings:Default").Get<string>();

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException("ConnectionStrings:Default не указан в настройках appsettings.json");
            }

            dbSettings.DbProviderType = dbProviderType;
            dbSettings.ConnectionString = connectionString;
            dbSettings.RetryAttemts = 2;

            return dbSettings;
        }
    }
}
