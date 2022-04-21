﻿using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using TelegramFatCamel.Module.Services.Interfaces;
using TelegramFatCamel.Module.StaticClasses;

namespace TelegramFatCamel.Module.Services
{
    public class TelegramFatCamelBotService : ITelegramFatCamelBotService
    {
        private TelegramBotSettings _telegramBotSettings;
        private TelegramBotClient _client;
        private CancellationTokenSource _cancellationToken;
        public TelegramFatCamelBotService(ITelegramSettingsService telegramSettingsService)
        {
            _telegramBotSettings = telegramSettingsService.GetSettings();
        }

        public Task FatCamelBotStartAsync()
        {
            StartTelegramBot();
            return Task.CompletedTask;
        }

        public Task FatCamelBotStopAsync()
        {
            StopTelegramBot();
            return Task.CompletedTask;
        }

        private void StartTelegramBot()
        {
            _client = new TelegramBotClient(_telegramBotSettings.Token);
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };

            _cancellationToken = new CancellationTokenSource();

            _client.StartReceiving(Handlers.HandleUpdateAsync,
                           Handlers.HandleErrorAsync,
                           receiverOptions,
                           _cancellationToken.Token);
        }

        private void StopTelegramBot()
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }
        }
    }
}