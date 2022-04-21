using Microsoft.Extensions.Configuration;
using System;

namespace TelegramFatCamel.Module.StaticClasses
{
    public class TelegramBotSettings
    {
        private static readonly Lazy<TelegramBotSettings> lazy = new(() => new TelegramBotSettings());

        public string Token { get; set; }

        private TelegramBotSettings()
        {
        }

        public static TelegramBotSettings GetInstance()
        {
            return lazy.Value;
        }

        public static TelegramBotSettings Initialize(IConfiguration configuration)
        {
            if (configuration != null)
            {
                return SetSettingsInfo(GetInstance(), configuration);
            }

            return GetInstance();
        }

        private static TelegramBotSettings SetSettingsInfo(TelegramBotSettings telegramSettings, IConfiguration configuration)
        {
            telegramSettings.Token = configuration.GetSection("Telegram:Token").Get<string>();

            return telegramSettings;
        }
    }
}