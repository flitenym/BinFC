using FatCamel.Host.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TelegramFatCamel.Module.Services;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module
{
    public class Startup : IModule
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);

            services.AddSingleton<ITelegramSettingsService, TelegramSettingsService>();
            services.AddSingleton<ITelegramFatCamelBotService, TelegramFatCamelBotService>();
        }
    }
}