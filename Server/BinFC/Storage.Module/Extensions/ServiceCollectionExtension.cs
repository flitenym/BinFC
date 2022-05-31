using FatCamel.Host.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Module.Export.Services;
using Storage.Module.Export.Services.Interfaces;
using Storage.Module.Import.Services;
using Storage.Module.Import.Services.Interfaces;
using Storage.Module.Repositories;
using Storage.Module.Repositories.Base;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.Services;
using Storage.Module.Services.Interfaces;

namespace Storage.Module.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddStorage(this IServiceCollection services, IConfiguration configuration)
        {
            DbSettings dbSettings = DbSettings.Initialize(configuration);

            if (dbSettings.DbProviderType == DbProviderType.SqlServer)
            {
                services.AddDbContext<DataContext>(
                    opt => opt.UseSqlServer(
                        dbSettings.ConnectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(dbSettings.RetryAttemts);
                        }
                    )
                );
            }
            else if (dbSettings.DbProviderType == DbProviderType.PostgreSql)
            {
                services.AddDbContext<DataContext>(
                    opt => opt.UseNpgsql(
                        dbSettings.ConnectionString,
                        npgsqlOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(dbSettings.RetryAttemts);
                        }
                    )
                );
            }
        }

        public static void AddStorageServices(this IServiceCollection services)
        {
            services.AddTransient<IDbSettingsService, DbSettingsService>();
            services.AddTransient<IInitialCreateService, InitialCreateService>();

            services.AddScoped<IPaymentService, PaymentService>();
        }

        public static void AddStorageRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<IUserInfoRepository, UserInfoRepository>();
            services.AddScoped<IUniqueRepository, UniqueRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<ISpotDataRepository, SpotDataRepository>();
            services.AddScoped<IFuturesDataRepository, FuturesDataRepository>();
            services.AddScoped<IFuturesScaleRepository, FuturesScaleRepository>();
            services.AddScoped<ISpotScaleRepository, SpotScaleRepository>();
            services.AddScoped<IPayHistoryRepository, PayHistoryRepository>();
            services.AddScoped<ITelegramUserInfoRepository, TelegramUserInfoRepository>();
            services.AddScoped<ITelegramMessageQueueRepository, TelegramMessageQueueRepository>();            
        }

        public static void AddStorageImportServices(this IServiceCollection services)
        {
            services.AddScoped<IImportService, ImportService>();
        }

        public static void AddStorageExportServices(this IServiceCollection services)
        {
            services.AddScoped<IExportPayHistoryService, ExportPayHistoryService>();
        }
    }
}