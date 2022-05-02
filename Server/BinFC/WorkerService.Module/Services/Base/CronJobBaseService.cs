using Cronos;
using Microsoft.Extensions.Logging;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.StaticClasses;
using System;
using System.Threading.Tasks;

namespace WorkerService.Module.Services.Base
{
    public abstract class CronJobBaseService<T>
    {
        private readonly ILogger _logger;
        private readonly ISettingsRepository _settingsRepository;
        private System.Timers.Timer _timer;

        public CronJobBaseService(ISettingsRepository settingsRepository, ILogger logger)
        {
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        public virtual async Task StartAsync()
        {
            await ScheduleJob();
        }

        public virtual async Task StopAsync()
        {
            _timer?.Stop();
            _timer?.Dispose();
            await Task.CompletedTask;
        }

        public virtual async Task RestartAsync()
        {
            await StopAsync();
            await StartAsync();
        }

        public virtual async Task DoWork()
        {
            await Task.Delay(5000);
        }

        private async Task ScheduleJob()
        {
            (bool isSuccess, string cronExpression) =
                await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.CronExpression, false);

            if (!isSuccess)
            {
                _logger.LogError("Не удалось запустить сервис продажи, т.к. не найдена cron запись в БД.");
                return;
            }

            CronExpression expression = CronExpression.Parse(cronExpression);

            var next = expression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Utc);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)
                {
                    await ScheduleJob();
                }
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    await DoWork();

                    await ScheduleJob();
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }
    }
}