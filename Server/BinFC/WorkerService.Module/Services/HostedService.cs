using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramFatCamel.Module.Services;
using WorkerService.Module.Localization;

namespace WorkerService.Module.Services
{
    public class HostedService : IHostedService, IDisposable
    {
        private readonly TelegramFatCamelBotService _telegramFatCamelBotService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HostedService> _logger;
        private Timer _timer = null;

        public HostedService(
            TelegramFatCamelBotService telegramFatCamelBotService, 
            IServiceScopeFactory scopeFactory, 
            ILogger<HostedService> logger)
        {
            _telegramFatCamelBotService = telegramFatCamelBotService;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(WorkerServiceLoc.StartHostedServer);

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(60),
                TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(WorkerServiceLoc.StopHostedServer);

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async void DoWork(object state)
        {
            try
            {
                // будем отправлять сообщения пользователям
                await SendMessagesAsync();

                // удалим userInfo, где не подтверждены и время больше недели.
                await NotApproveUserInfoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
            }
        }

        private async Task SendMessagesAsync()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    ITelegramMessageQueueRepository telegramMessageQueueRepository =
                        scope.ServiceProvider
                            .GetService<ITelegramMessageQueueRepository>();

                    IEnumerable<TelegramMessageQueue> messagesQueue = telegramMessageQueueRepository.Get();

                    if (!messagesQueue.Any())
                    {
                        return;
                    }

                    TelegramBotClient _client = await _telegramFatCamelBotService.GetTelegramBotAsync(false);

                    foreach (var messageQueue in messagesQueue)
                    {
                        await _client.SendTextMessageAsync(messageQueue.ChatId, messageQueue.Message);

                        _logger.LogTrace(string.Format(WorkerServiceLoc.SendNotificationText, messageQueue.ChatId, messageQueue.Message));

                        await telegramMessageQueueRepository.DeleteAsync(messageQueue.Id);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, WorkerServiceLoc.SendNotificationError);
            }            
        }

        private async Task NotApproveUserInfoAsync()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    IUserInfoRepository userInfoRepository =
                        scope.ServiceProvider
                            .GetService<IUserInfoRepository>();

                    IEnumerable<UserInfo> needToNotApprove = userInfoRepository.Get(DateTime.UtcNow.AddDays(-7));

                    if (!needToNotApprove.Any())
                    {
                        return;
                    }

                    await userInfoRepository.NotApproveAsync(needToNotApprove.Select(x => x.Id));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, WorkerServiceLoc.NotApproveUserInfo);
            }
        }
    }
}