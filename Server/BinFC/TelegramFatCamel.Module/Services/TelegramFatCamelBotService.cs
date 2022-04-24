using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Services
{
    public class TelegramFatCamelBotService : ITelegramFatCamelBotService
    {
        private readonly ITelegramSettingsService _telegramSettingsService;

        private readonly ICommandExecutorService _commandExecutorService;

        private TelegramBotClient _client;

        private CancellationTokenSource _cancellationToken;

        public TelegramFatCamelBotService(ITelegramSettingsService telegramSettingsService, ICommandExecutorService commandExecutorService)
        {
            _telegramSettingsService = telegramSettingsService;
            _commandExecutorService = commandExecutorService;
        }

        public async Task<TelegramBotClient> GetTelegramBotAsync()
        {
            if (_client != null)
            {
                return _client;
            }

            await StartTelegramBotAsync();

            return _client;
        }

        private Task StartTelegramBotAsync()
        {
            _client = new TelegramBotClient(_telegramSettingsService.Token);
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };

            _cancellationToken = new CancellationTokenSource();

            _client.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                _cancellationToken.Token);

            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await _commandExecutorService.Execute(update);
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public Task StopTelegramBotAsync()
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }

            return Task.CompletedTask;
        }
    }
}