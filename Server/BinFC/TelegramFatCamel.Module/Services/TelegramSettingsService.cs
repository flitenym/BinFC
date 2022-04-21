using Microsoft.Extensions.Configuration;
using TelegramFatCamel.Module.Services.Interfaces;
using TelegramFatCamel.Module.StaticClasses;

namespace TelegramFatCamel.Module.Services
{
    public class TelegramSettingsService : ITelegramSettingsService
    {
        public TelegramSettingsService(IConfiguration configuration)
        {
            TelegramBotSettings.Initialize(configuration);
        }

        public TelegramBotSettings GetSettings()
        {
            return TelegramBotSettings.GetInstance();
        }
    }
}