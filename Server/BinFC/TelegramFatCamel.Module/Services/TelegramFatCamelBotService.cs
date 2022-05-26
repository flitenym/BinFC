using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    public class TelegramFatCamelBotService : ITelegramFatCamelBotService, IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<TelegramFatCamelBotService> _logger;

        private TelegramBotClient _client;

        private bool IsHandlersStarted = false;

        private string _lastCommand = null;

        private CancellationTokenSource _cancellationToken;

        private string Token { get; set; }

        public TelegramFatCamelBotService(
            IServiceProvider serviceProvider,
            ILogger<TelegramFatCamelBotService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            Token = configuration.GetSection("Telegram:Token").Get<string>();
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
            _client = new TelegramBotClient(Token);
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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<ICommandExecutorService>();

                    _lastCommand = await scopedProcessingService.ExecuteAsync(botClient, _lastCommand, update);
                }
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
                ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError(ErrorMessage);
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await GetTelegramBotAsync(true);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopTelegramBotAsync();
        }

        public void Dispose()
        {

        }
    }
}