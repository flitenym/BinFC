using Binance.Net.Objects.Models.Spot;
using BinanceApi.Module.Classes;
using BinanceApi.Module.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Storage.Module.Classes;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.StaticClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramFatCamel.Module.Services;
using TelegramFatCamel.Module.Services.Interfaces;
using WorkerService.Module.Services.Base;
using WorkerService.Module.Services.Intrefaces;

namespace WorkerService.Module.Services
{
    public class BinanceSellService : CronJobBaseService<IBinanceSellService>
    {
        private const int AttemptsToSellCurrensies = 3;

        private readonly TelegramFatCamelBotService _telegramFatCamelBotService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBinanceApiService _binanceApiService;
        private readonly ILogger<BinanceSellService> _logger;

        public BinanceSellService(
            TelegramFatCamelBotService telegramFatCamelBotService,
            IServiceScopeFactory scopeFactory,
            IBinanceApiService binanceApiService,
            IConfiguration configuration,
            ILogger<BinanceSellService> logger) :
            base(scopeFactory, configuration, logger)
        {
            _telegramFatCamelBotService = telegramFatCamelBotService;
            _scopeFactory = scopeFactory;
            _binanceApiService = binanceApiService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Запуск службы {nameof(BinanceSellService)}");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger?.LogTrace($"Остановка службы {nameof(BinanceSellService)}");
            return base.StopAsync(cancellationToken);
        }

        public override Task RestartAsync(CancellationToken cancellationToken)
        {
            _logger?.LogTrace($"Перезапуск службы {nameof(BinanceSellService)}");
            return base.RestartAsync(cancellationToken);
        }

        public override async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Запуск продажи");
            try
            {
                var settings = await GetSettingsInfoAsync();
                (bool isSuccess, string sellMessage) = await SellAsync(settings);
                await SendTelegramMessageAsync(settings, sellMessage);
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
                string error = $"Продажа прошла с ошибками {ex}";
                _logger?.LogInformation(ex, error);
            }
        }

        private async Task<SettingsInfo> GetSettingsInfoAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                ISettingsRepository _settingsRepository =
                    scope.ServiceProvider
                        .GetRequiredService<ISettingsRepository>();

                var _apiKey = await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.ApiKey, false);
                var _apiSecret = await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.ApiSecret, false);
                var _cronExpression = await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.CronExpression, false);
                var _sellCurrency = await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.SellCurrency, false);
                var _isNotification = await _settingsRepository.GetSettingsByKeyAsync<bool>(SettingsKeys.IsNotification, false);
                var _binanceSellEnable = await _settingsRepository.GetSettingsByKeyAsync<bool>(SettingsKeys.BinanceSellEnable, false);

                return new SettingsInfo()
                {
                    ApiKey = _apiKey.Value,
                    ApiSecret = _apiSecret.Value,
                    CronExpression = _cronExpression.Value,
                    SellCurrency = _sellCurrency.Value,
                    IsNotification = _isNotification.Value,
                    BinanceSellEnable = _binanceSellEnable.Value,
                };
            }
        }

        private async Task<(bool isSuccess, string message)> SellAsync(SettingsInfo settings)
        {
            (bool isValid, string validError) = settings.IsValid();

            if (!isValid)
            {
                return (false, validError);
            }

            await TransferFuturesToSpotAsync(settings);

            await SellCurrenciesAsync(settings);

            return (true, null);
        }

        #region Продажа и перевод крипты

        /// <summary>
        /// Перевод фьючи в спот.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private async Task<bool> TransferFuturesToSpotAsync(SettingsInfo settings)
        {
            _logger.LogTrace("Начинаем перевод USDT из фьючерс в спот");

            bool isSuccessTransferSpot = await _binanceApiService.TransferFuturesToSpotUSDTAsync(settings);

            if (isSuccessTransferSpot)
            {
                _logger.LogTrace($"Перевод USDT из фьючерс в спот был осуществлен");
                return true;
            }
            else
            {
                _logger.LogError($"Перевод USDT из фьючерс в спот не был осуществлен");
                return false;
            }
        }

        /// <summary>
        /// Продажа крипты.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private async Task<bool> SellCurrenciesAsync(SettingsInfo settings)
        {
            (bool isSuccessGetExchangeInfo, BinanceExchangeInfo exchangeInfo) = await _binanceApiService.GetExchangeInfoAsync(settings);

            if (!isSuccessGetExchangeInfo || exchangeInfo == null)
            {
                _logger.LogError("Ошибка получения информации минимальных требований по символам для перевода.");
                return false;
            }

            // получим все валюты в балансе аккаунта, кроме USDT и BNB
            (bool isSuccessGetAllCurrencies, List<BinanceBalance> currencies) = await _binanceApiService.GetBinanceCurrenciesAsync(settings, new List<string>() { "USDT", "BNB" });

            if (!isSuccessGetAllCurrencies || currencies == null)
            {
                return false;
            }

            List<CurrencyInfo> currenciesInfo = new(currencies.Select(x => new CurrencyInfo(x.Asset, false, false)));

            _logger.LogTrace($"Начинаем продажу монет {string.Join(", ", currenciesInfo.Select(x => x.Asset))}");

            #region Продажа криптовалют

            for (int i = 0; i < AttemptsToSellCurrensies; i++)
            {
                if (!currenciesInfo.Where(x => !x.IsSuccess).Any())
                {
                    break;
                }

                // пройдем по всем валютам, которые есть на аккаунте и попробуем продать их
                for (int j = 0; j < currenciesInfo.Count; j++)
                {
                    if (!currenciesInfo[j].IsSuccess)
                    {
                        (bool isSuccessSellCoin, bool isSellCoin, bool isDustSellCoin) = await SellCoinAsync(currenciesInfo[j].Asset, exchangeInfo, settings: settings);

                        currenciesInfo[j].IsSell = isSellCoin;
                        currenciesInfo[j].IsDust = isDustSellCoin;
                    }
                }
            }

            _logger.LogTrace("Продажа монет выполнена");

            #endregion

            #region Перевод монет с маленьким балансом в BNB

            _logger.LogTrace("Начинаем перевод монет с маленьким балансом в BNB");

            // перевод мелких монет в BNB
            bool isSuccessTransferDust = await _binanceApiService.TransferDustAsync(currenciesInfo.Where(x => x.IsDust).Select(x => x.Asset).ToList(), settings: settings);

            if (isSuccessTransferDust)
            {
                _logger.LogTrace("Перевод монет с маленьким балансом в BNB закончен");
            }
            else
            {
                _logger.LogTrace("Перевод монет с маленьким балансом в BNB неудачен");
            }

            #endregion

            (bool isSuccessSellBNB, bool isSellCoinBNB, bool isDustSellBNB) = await SellCoinAsync("BNB", exchangeInfo, settings: settings);

            return true;
        }

        /// <summary>
        /// Продажа одной валюты.
        /// </summary>
        /// <param name="currencyAsset"></param>
        /// <param name="exchangeInfo"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private async Task<(bool IsSuccess, bool IsSell, bool IsDust)> SellCoinAsync(string currencyAsset, BinanceExchangeInfo exchangeInfo, SettingsInfo settings)
        {
            _logger.LogTrace($"Продажа {currencyAsset}");

            // получим валюту и определим пыль или нет, если нет, то сразу продадим ее
            (bool isSuccessCurrency, AssetsInfo currencyInfo) = await _binanceApiService.GetСurrencyAsync(exchangeInfo, currencyAsset, settings: settings);

            if (!isSuccessCurrency || currencyInfo == default || string.IsNullOrEmpty(currencyInfo.FromAsset) || string.IsNullOrEmpty(currencyInfo.ToAsset))
            {
                _logger.LogTrace($"Продажа {currencyAsset}: неудачное получение валюты.");
                return default;
            }

            if (!currencyInfo.IsDust)
            {
                _logger.LogTrace($"Продажа {currencyAsset}: выполним продажу.");

                bool isSuccessSell = await _binanceApiService.SellCoinAsync(currencyInfo.Quantity, currencyInfo.FromAsset, currencyInfo.ToAsset, settings: settings);

                _logger.LogTrace($"Продажа {currencyAsset}:{(isSuccessSell ? "" : " не")} выполнилась продажа по {currencyInfo.ToAsset}.");

                return (true, isSuccessSell, false);
            }

            return (true, false, true);
        }

        #endregion

        #region Отправка уведолмения в телеграме

        private async Task SendTelegramMessageAsync(SettingsInfo settings, string message)
        {
            _logger.LogTrace(message);
            if (!settings.IsNotification)
            {
                _logger.LogTrace("Уведомления отключены.");
                return;
            }

            List<UserInfo> admins;

            using (var scope = _scopeFactory.CreateScope())
            {
                IUserInfoRepository _userInfoRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IUserInfoRepository>();

                admins = await _userInfoRepository.GetAdminsAsync();
            }

            if (!admins.Any())
            {
                _logger.LogTrace("Администраторов не найдено для отправки уведомления.");
                return;
            }

            TelegramBotClient _client = await _telegramFatCamelBotService.GetTelegramBotAsync(false);

            foreach (var admin in admins)
            {
                await _client.SendTextMessageAsync(admin.ChatId, message);

                _logger.LogTrace($"Отправлено уведомление пользователю {admin.UserName} с chatId {admin.ChatId}: {message}.");
            }
        }

        #endregion
    }
}