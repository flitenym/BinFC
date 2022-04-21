using Microsoft.Extensions.DependencyInjection;
using TelegramFatCamel.Module.Services;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramSettingsService, TelegramSettingsService>();
            services.AddSingleton<ITelegramFatCamelBotService, TelegramFatCamelBotService>();
        }
    }
}
