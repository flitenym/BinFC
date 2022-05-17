using Cronos;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private System.Timers.Timer _timer;

        public CronJobBaseService(ISettingsRepository settingsRepository, IConfiguration configuration, ILogger logger)
        {
            _settingsRepository = settingsRepository;
            _configuration = configuration;
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

        public virtual async Task<string> DoWorkAsync()
        {
            await Task.Delay(5000);
            return null;
        }

        private async Task<string> ScheduleJob()
        {
            string sheduleJobError = null;
            string workError = null;

            if (!string.IsNullOrEmpty(sheduleJobError))
            {
                _logger.LogError(sheduleJobError);
                return workError;
            }

            if (!string.IsNullOrEmpty(workError))
            {
                _logger.LogError(workError);
                return workError;
            }

            (bool isSuccess, string cronExpression) =
                await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.CronExpression, false);

            if (!isSuccess)
            {
                string error = "Не удалось запустить сервис продажи, т.к. не найдена cron запись в БД.";
                _logger.LogError(error);
                return error;
            }

            cronExpression ??= _configuration.GetSection("Cron:Expression").Get<string>();

            if (string.IsNullOrEmpty(cronExpression))
            {
                string error = "Не удалось получить cron запись.";
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
                    sheduleJobError = await ScheduleJob();

                    if (!string.IsNullOrEmpty(sheduleJobError))
                    {
                        return sheduleJobError;
                    }
                }

                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    workError = await DoWorkAsync();

                    if (!string.IsNullOrEmpty(workError))
                    {
                        _timer?.Stop();
                        return;
                    }

                    await ScheduleJob();
                };
                _timer?.Start();
            }

            return await Task.FromResult<string>(null);
        }
    }
}