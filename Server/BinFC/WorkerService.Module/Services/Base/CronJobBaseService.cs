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

        public virtual async Task<string> StartAsync()
        {
            return await ScheduleJob();
        }

        public virtual Task<string> StopAsync()
        {
            _timer?.Stop();
            _timer?.Dispose();

            return Task.FromResult<string>(null);
        }

        public virtual async Task<string> RestartAsync()
        {
            string stopError = await StopAsync();
            if (!string.IsNullOrEmpty(stopError))
            {
                return stopError;
            }

            return await StartAsync();
        }

        public virtual async Task<string> DoWork()
        {
            await Task.Delay(5000);
            return null;
        }

        private async Task<string> ScheduleJob()
        {
            (bool isSuccess, string cronExpression) =
                await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.CronExpression, false);

            if (!isSuccess)
            {
                string error = "Не удалось запустить сервис продажи, т.к. не найдена cron запись в БД.";
                _logger.LogError(error);
                return error;
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

            return await Task.FromResult<string>(null);
        }
    }
}