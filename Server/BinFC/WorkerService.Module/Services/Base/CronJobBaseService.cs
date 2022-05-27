using Cronos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.StaticClasses;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkerService.Module.Cronos;

namespace WorkerService.Module.Services.Base
{
    public abstract class CronJobBaseService<T> : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IConfiguration _configuration;
        private System.Timers.Timer _timer;
        private CronExpression _expression;

        public CronJobBaseService(ISettingsRepository settingsRepository, IConfiguration configuration, ILogger logger)
        {
            _settingsRepository = settingsRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await ScheduleJobAsync(cancellationToken);
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            return Task.CompletedTask;
        }

        public virtual async Task RestartAsync(CancellationToken cancellationToken)
        {
            await StopAsync(cancellationToken);
            await StartAsync(cancellationToken);
        }

        public virtual async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);
        }

        protected virtual async Task ScheduleJobAsync(CancellationToken cancellationToken)
        {
            (bool isSuccess, string cronExpression) =
                await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.CronExpression, false);

            if (!isSuccess)
            {
                string error = "Не удалось запустить сервис продажи, т.к. не найдена cron запись в БД.";
                _logger.LogError(error);
                return;
            }

            cronExpression ??= _configuration.GetSection("Cron:Expression").Get<string>();

            _expression = CronParseHelper.GetCronExpression(cronExpression);

            if (_expression == null)
            {
                string error = "Не удалось получить cron запись.";
                _logger.LogError(error);
                return;
            }

            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Utc);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                {
                    await ScheduleJobAsync(cancellationToken);
                }
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();  // reset and dispose timer
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await DoWorkAsync(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJobAsync(cancellationToken);    // reschedule next
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }
    }
}