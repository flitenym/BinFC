﻿using FatCamel.Host.Enum;
using FatCamel.Host.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Module.Services;
using Storage.Module.Services.Interfaces;

namespace Storage.Module
{
    public class Startup : IModule
    {
        private IConfiguration _configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            DbSettings dbSettings = DbSettings.Initialize(_configuration);

            if (dbSettings.DbProviderType == DbProviderType.SqlServer)
            {
                services.AddDbContext<DataContext>(opt => opt.UseSqlServer(dbSettings.ConnectionString), ServiceLifetime.Singleton);
            }
            else if (dbSettings.DbProviderType == DbProviderType.PostgreSql)
            {
                services.AddDbContext<DataContext>(opt => opt.UseNpgsql(dbSettings.ConnectionString), ServiceLifetime.Singleton);
            }

            services.AddScoped<IDbSettingsService, DbSettingsService>();
        }
    }
}