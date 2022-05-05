using Binance.Net.Objects.Models.Spot;
using BinanceApi.Module.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Storage.Module.Classes;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.StaticClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkerService.Module.Services.Base;
using WorkerService.Module.Services.Intrefaces;

namespace WorkerService.Module.Services
{
    public class BinanceSell : CronJobBaseService<IBinanceSell>
    {
        private readonly IBinanceApiService _binanceApiService;
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger<BinanceSell> _logger;

        public BinanceSell(IBinanceApiService binanceApiService, ISettingsRepository settingsRepository, IConfiguration configuration, ILogger<BinanceSell> logger) :
            base(settingsRepository, configuration, logger)
        {
            _binanceApiService = binanceApiService;
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
                (bool isSuccess, string sellError) = await SellAsync();
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

        private async Task<(bool isSuccess, string error)> SellAsync()
        {
            SettingsInfo settings = new SettingsInfo()
            {
                ApiKey = (await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.ApiKey, false)).Value,
                ApiSecret = (await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.ApiSecret, false)).Value,
                Emails = (await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.Emails, false)).Value,
                EmailLogin = (await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.EmailLogin, false)).Value,
                EmailPassword = (await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.EmailPassword, false)).Value,
                CronExpression = (await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.CronExpression, false)).Value,
                SellCurrency = (await _settingsRepository.GetSettingsByKeyAsync<string>(SettingsKeys.SellCurrency, false)).Value
            };

            (bool isValid, string validError) = settings.IsValid();

            if (!isValid)
            {
                return (false, validError);
            }

            bool transferIsSuccess = await TransferFuturesToSpotAsync(settings);



            return (true, null);
        }

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

        private async Task<bool> SellAsync(SettingsInfo settings)
        {
            List<string> finalCoins = new List<string>();

            (bool isSuccessGetExchangeInfo, BinanceExchangeInfo exchangeInfo) = await _binanceApiService.GetExchangeInfoAsync(settings);

            if (!isSuccessGetExchangeInfo || exchangeInfo == null)
            {
                _logger.LogError("Ошибка получения информации минимальных требований по символам для перевода");
                return false;
            }

            // получим все валюты в балансе аккаунта, кроме USDT и BNB
            (bool isSuccessGetAllCurrencies, List<BinanceBalance> currencies) = await _binanceApiService.GetBinanceCurrenciesAsync(settings, new List<string>() { "USDT", "BNB" });

            if (!isSuccessGetAllCurrencies || currencies == null)
            {
                return false;
            }

            #region Продажа криптовалют

            _logger.LogTrace($"Начинаем продажу монет {string.Join(", ", currencies.Select(x => x.Asset))}");
            // пройдем по всем валютам, которые есть на аккаунте и попробуем продать их
            foreach (var currency in currencies)
            {
                var finalCoin = await SellCoin(currency.Asset, exchangeInfo, settings: settings);
                if (!string.IsNullOrEmpty(finalCoin) && finalCoin != "USDT" && !finalCoins.Contains(finalCoin))
                {
                    finalCoins.Add(finalCoin);
                }
            }
            _logger.LogTrace("Продажа монет выполнена");

            #endregion

            #region Перевод монет с маленьким балансом в BNB

            _logger.LogTrace("Начинаем перевод монет с маленьким балансом в BNB");
            if (!finalCoins.Contains("BNB"))
            {
                finalCoins.Add("BNB");
            }

            (bool isSuccessGetAllAfterSellCurrencies, List<(string asset, decimal quantity, bool isDust)> currenciesAfterSell) = 
                await _binanceApiService.GetAllCurrenciesWithoutUSDTWithQuantityAsync(exchangeInfo, settings: settings);

            if (isSuccessGetAllAfterSellCurrencies)
            {
                // перевод мелких монет в BNB
                (bool isSuccessTransferDust, string messageTransferDust) = await _binanceApiService.TransferDustAsync(currenciesAfterSell, settings: settings);

                if (!string.IsNullOrEmpty(messageTransferDust))
                {
                    _logger.LogTrace(messageTransferDust);
                }
            }

            _logger.LogTrace("Перевод монет с маленьким балансом в BNB закончен");

            #endregion

            return true;
        }

        private async Task<string> SellCoin(string currencyAsset, BinanceExchangeInfo exchangeInfo, SettingsInfo settings)
        {
            _logger.LogTrace($"Продажа {currencyAsset}");

            if (string.IsNullOrEmpty(currencyAsset))
            {
                return null;
            }

            // получим валюту и определим пыль или нет, если нет, то сразу продадим ее
            (bool isSuccessCurrency, (string fromAsset, string toAsset, decimal quantity, bool isDust) currencyInfo) =
                await _binanceApiService.GetСurrencyAsync(exchangeInfo, currencyAsset, settings: settings);

            if (!isSuccessCurrency || currencyInfo == default || string.IsNullOrEmpty(currencyInfo.fromAsset) || string.IsNullOrEmpty(currencyInfo.toAsset))
            {
                _logger.LogTrace($"Продажа {currencyAsset}: неудачное получение валюты.");
                return null;
            }

            if (!currencyInfo.isDust)
            {
                _logger.LogTrace($"Продажа {currencyAsset}: выполним продажу.");

                bool isSuccessSell = await _binanceApiService.SellCoinAsync(currencyInfo.quantity, currencyInfo.fromAsset, currencyInfo.toAsset, settings: settings);

                _logger.LogTrace($"Продажа {currencyAsset}:{(isSuccessSell ? "" : " не")} выполнилась продажа по {currencyInfo.toAsset}.");
            }

            return currencyInfo.toAsset;
        }
    }
}