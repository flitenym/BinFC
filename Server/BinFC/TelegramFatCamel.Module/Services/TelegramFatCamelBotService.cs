using Microsoft.Extensions.DependencyInjection;
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

        private readonly IServiceProvider _serviceProvider;

        private ICommandExecutorService _commandExecutorService;

        private TelegramBotClient _client;

        private bool IsHandlersStarted = false;

        private CancellationTokenSource _cancellationToken;

        public TelegramFatCamelBotService(IServiceProvider serviceProvider, ITelegramSettingsService telegramSettingsService)
        {
            _serviceProvider = serviceProvider;
            _telegramSettingsService = telegramSettingsService;
        }

        public async Task<TelegramBotClient> GetTelegramBotAsync(bool isNeedHandlers = true)
        {
            if (_client != null)
            {
                if (isNeedHandlers && !IsHandlersStarted)
                {
                    await StartTelegramBotAsync();
                }

                return _client;
            }

            if (isNeedHandlers)
            {
                CreateTelegram();
                await StartTelegramBotAsync();
            }
            else
            {
                CreateTelegram();
            }

            return _client;
        }

        private void CreateTelegram()
        {
            _client = new TelegramBotClient(_telegramSettingsService.Token);
        }

        private Task StartTelegramBotAsync()
        {
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };

            _cancellationToken = new CancellationTokenSource();

            _client.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                _cancellationToken.Token);

            IsHandlersStarted = true;

            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                _commandExecutorService ??= _serviceProvider.GetRequiredService<ICommandExecutorService>();
                await _commandExecutorService.ExecuteAsync(update);
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