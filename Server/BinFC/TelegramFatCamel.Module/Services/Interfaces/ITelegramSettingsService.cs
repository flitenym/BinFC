using TelegramFatCamel.Module.StaticClasses;

namespace TelegramFatCamel.Module.Services.Interfaces
{
    public interface ITelegramSettingsService
    {
        public TelegramBotSettings GetSettings();
    }
}