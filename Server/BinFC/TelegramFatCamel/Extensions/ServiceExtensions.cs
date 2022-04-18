using Microsoft.Extensions.DependencyInjection;
using TelegramFatCamel.Services;

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
