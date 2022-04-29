using BinanceApi.Module.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Storage.Module.Repositories.Interfaces;

namespace BinanceApi.Module.Services
{
    public class BinanceApiService : IBinanceApiService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger<BinanceApiService> _logger;
        public BinanceApiService(ISettingsRepository settingsRepository, ILogger<BinanceApiService> logger)
        {
            _settingsRepository = settingsRepository;
            _logger = logger;
        }
    }
}