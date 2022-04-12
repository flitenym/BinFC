using FatCamel.Host.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FatCamel.Host.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramFatCamelBotService, TelegramFatCamelBotService>();
        }
    }
}
