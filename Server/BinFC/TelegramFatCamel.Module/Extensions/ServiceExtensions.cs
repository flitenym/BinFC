using Microsoft.Extensions.DependencyInjection;
using TelegramFatCamel.Module.Services;

namespace TelegramFatCamel.Module.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramFatCamelBotService, TelegramFatCamelBotService>();
        }
    }
}
