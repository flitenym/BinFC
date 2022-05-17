using FatCamel.Host.Enum;
using FatCamel.Host.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Module.Repositories;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.Services;
using Storage.Module.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Storage.Module
{
    public class Startup : IModule
    {
        private IConfiguration _configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ConfigureAsync(IApplicationBuilder app, IHostApplicationLifetime hal, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            var initialCreate = serviceProvider.GetRequiredService<IInitialCreateService>();
            await initialCreate.InitialCreateValuesAsync();
        }

        public Task ConfigureServicesAsync(IServiceCollection services)
        {
            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);

            DbSettings dbSettings = DbSettings.Initialize(_configuration);

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

            services.AddScoped<IDbSettingsService, DbSettingsService>();
            services.AddScoped<IInitialCreateService, InitialCreateService>();
            services.AddScoped<IUserInfoRepository, UserInfoRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();

            return Task.CompletedTask;
        }
    }
}