using BinanceApi.Module.Services;
using BinanceApi.Module.Services.Interfaces;
using FatCamel.Host.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace BinanceApi.Module
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

            services.AddSingleton<IBinanceApiService, BinanceApiService>();

            services.AddScoped<IPaymentService, PaymentService>();

            return Task.CompletedTask;
        }
    }
}