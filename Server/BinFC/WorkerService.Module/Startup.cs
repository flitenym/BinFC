﻿using FatCamel.Host.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using TelegramFatCamel.Module.Services;
using TelegramFatCamel.Module.Services.Interfaces;
using WorkerService.Module.Services;
using WorkerService.Module.Services.Base;
using WorkerService.Module.Services.Intrefaces;

namespace WorkerService.Module
{
    public class Startup : IModule
    {
        public Task ConfigureAsync(IApplicationBuilder app, IHostApplicationLifetime hal, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
        }

        public Task ConfigureServicesAsync(IServiceCollection services)
        {
            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);

            services.AddScoped<CronJobBaseService<IBinanceSell>, BinanceSell>();

            services.AddScoped<ITelegramFatCamelBotService, TelegramFatCamelBotService>();

            return Task.CompletedTask;
        }
    }
}