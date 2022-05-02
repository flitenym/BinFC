using Microsoft.Extensions.Logging;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using WorkerService.Module.Services.Base;
using WorkerService.Module.Services.Intrefaces;

namespace WorkerService.Module.Services
{
    public class BinanceSell : CronJobBaseService<IBinanceSell>
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger<BinanceSell> _logger;

        public BinanceSell(ISettingsRepository settingsRepository, ILogger<BinanceSell> logger) : base(settingsRepository, logger)
        {
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        public override async Task StartAsync()
        {
            _logger.LogTrace($"Запуск службы {nameof(BinanceSell)}");
            await base.StartAsync();
        }

        public override Task StopAsync()
        {
            _logger?.LogTrace($"Остановка службы {nameof(BinanceSell)}");
            return base.StopAsync();
        }

        public override Task RestartAsync()
        {
            _logger?.LogTrace($"Перезапуск службы {nameof(BinanceSell)}");
            return base.RestartAsync();
        }

        public override async Task DoWork()
        {
            _logger.LogTrace($"Запуск продажи");
            try
            {
                bool isSuccess = await Sell();
                if (isSuccess)
                {
                    _logger?.LogTrace($"Продажа прошла успешно");
                }
                else
                {
                    _logger?.LogInformation($"Продажа прошла неудачно");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogInformation(ex, $"Продажа прошла с ошибками {ex}");
            }

            return;
        }

        private async Task<bool> Sell()
        {
            return true;
        }
    }
}