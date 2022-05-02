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

        public override Task<string> StartAsync()
        {
            _logger.LogTrace($"Запуск службы {nameof(BinanceSell)}");
            return base.StartAsync();
        }

        public override Task<string> StopAsync()
        {
            _logger?.LogTrace($"Остановка службы {nameof(BinanceSell)}");
            return base.StopAsync();
        }

        public override Task<string> RestartAsync()
        {
            _logger?.LogTrace($"Перезапуск службы {nameof(BinanceSell)}");
            return base.RestartAsync();
        }

        public override async Task<string> DoWork()
        {
            _logger.LogTrace($"Запуск продажи");
            try
            {
                (bool isSuccess, string sellError) = await Sell();
                if (isSuccess)
                {
                    _logger?.LogTrace($"Продажа прошла успешно");
                    return null;
                }
                else
                {
                    _logger?.LogInformation($"Продажа прошла неудачно");
                    return sellError;
                }
            }
            catch (Exception ex)
            {
                string error = $"Продажа прошла с ошибками {ex}";
                _logger?.LogInformation(ex, error);
                return error;
            }
        }

        private async Task<(bool isSuccess, string error)> Sell()
        {
            return (true, null);
        }
    }
}