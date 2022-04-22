using Microsoft.Extensions.Configuration;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Services
{
    public class TelegramSettingsService : ITelegramSettingsService
    {
        public string Token { get; set; }

        public TelegramSettingsService(IConfiguration configuration)
        {
            Token = configuration.GetSection("Telegram:Token").Get<string>();
        }
    }
}