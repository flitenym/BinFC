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
using WorkerService.Module.Services.Base;
using WorkerService.Module.Services.Intrefaces;

namespace WorkerService.Module.Services
{
    public class NotificationResult
    {
        public List<string> NotificationMessages { get; set; } = new();
        public List<string> SuccessCoins { get; set; } = new();

        public string GetMessage()
        {
            string result = string.Join(Environment.NewLine, NotificationMessages.Distinct()).Trim();
            if (SuccessCoins.Any())
            {
                result += Environment.NewLine + string.Join("; ", SuccessCoins).Trim();
            }

            return result;
        }
    }

    public class BinanceSellService : CronJobBaseService<IBinanceSellService>
    {
        private const int AttemptsToSellCurrensies = 3;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBinanceApiService _binanceApiService;
        private readonly ILogger<BinanceSellService> _logger;

        private NotificationResult NotificationResult { get; set; }

        public BinanceSellService(
            IServiceScopeFactory scopeFactory,
            IBinanceApiService binanceApiService,
            IConfiguration configuration,
            ILogger<BinanceSellService> logger) :
            base(scopeFactory, configuration, logger)
        {
            _scopeFactory = scopeFactory;
            _binanceApiService = binanceApiService;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Запуск службы {nameof(BinanceSellService)}");
            await base.StartAsync(cancellationToken);
            await SetBinanceSellEnableAsync(true);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger?.LogTrace($"Остановка службы {nameof(BinanceSellService)}");
            await base.StopAsync(cancellationToken);
            await SetBinanceSellEnableAsync(false);
        }

        private async Task SetBinanceSellEnableAsync(bool isEnable)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                ISettingsRepository settingsRepository =
                    scope.ServiceProvider
                        .GetRequiredService<ISettingsRepository>();

                var binanceSellEnable = await settingsRepository.SetSettingsByKeyAsync(SettingsKeys.BinanceSellEnable, isEnable);

                await settingsRepository.SaveChangesAsync();
            }
        }

        public override Task RestartAsync(CancellationToken cancellationToken)
        {
            _logger?.LogTrace($"Перезапуск службы {nameof(BinanceSellService)}");
            return base.RestartAsync(cancellationToken);
        }

        public override async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Запуск продажи");

            NotificationResult = new NotificationResult();

            try
            {
                var settings = await GetSettingsInfoAsync();
                try
                {
                    bool isSuccess = await SellAsync(settings);
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
                    NotificationResult.NotificationMessages.Add(error);
                    _logger?.LogError(ex, error);
                }

                await SendTelegramMessageAsync(settings);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.StackTrace);
            }
        }

        private async Task<SettingsInfo> GetSettingsInfoAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                ISettingsRepository settingsRepository =
                    scope.ServiceProvider
                        .GetRequiredService<ISettingsRepository>();

                SettingsInfo settings = await settingsRepository.GetSettingsAsync();

                var notificationNameList = settings.GetNotificationNames(settings.NotificationNames);

                if (notificationNameList.Any())
                {
                    IUserInfoRepository userInfoRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IUserInfoRepository>();

                    settings.AdminsChatId = userInfoRepository.GetChatIdByUserNickName(notificationNameList);
                }

                return settings;
            }
        }

        private async Task<bool> SellAsync(SettingsInfo settings)
        {
            (bool isValid, string validError) = settings.IsValid();

            if (!isValid)
            {
                NotificationResult.NotificationMessages.Add(validError);
                return false;
            }

            return
                await TransferFuturesToSpotAsync(settings) & 
                await SellCurrenciesAsync(settings);
        }

        #region Продажа и перевод крипты

        /// <summary>
        /// Перевод фьючи в спот.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private async Task<bool> TransferFuturesToSpotAsync(SettingsInfo settings)
        {
            _logger.LogTrace($"Начинаем перевод {settings.SellCurrency} из фьючерс в спот");

            (bool isSuccessTransferSpot, string messageTransferSpot) = await _binanceApiService.TransferFuturesToSpotUSDTAsync(settings);

            if (isSuccessTransferSpot)
            {
                NotificationResult.NotificationMessages.Add($"Перевод {settings.SellCurrency} из фьючерс в спот был осуществлен. {messageTransferSpot}");
                _logger.LogTrace($"Перевод {settings.SellCurrency} из фьючерс в спот был осуществлен. {messageTransferSpot}");
                return true;
            }
            else
            {
                NotificationResult.NotificationMessages.Add($"Перевод {settings.SellCurrency} из фьючерс в спот не был осуществлен. {messageTransferSpot}");
                _logger.LogError($"Перевод {settings.SellCurrency} из фьючерс в спот не был осуществлен. {messageTransferSpot}");
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
            (bool isSuccessGetExchangeInfo, string messageGetExchangeInfo, BinanceExchangeInfo exchangeInfo) = await _binanceApiService.GetExchangeInfoAsync(settings);

            if (!isSuccessGetExchangeInfo || exchangeInfo == null)
            {
                NotificationResult.NotificationMessages.Add($"Ошибка получения информации минимальных требований по символам для перевода. {messageGetExchangeInfo}");
                _logger.LogError("Ошибка получения информации минимальных требований по символам для перевода.");
                return false;
            }

            // получим все валюты в балансе аккаунта, кроме USDT и BNB
            (bool isSuccessGetAllCurrencies, string messageGetAllCurrencies, List<BinanceBalance> currencies) = await _binanceApiService.GetBinanceCurrenciesAsync(settings, new List<string>() { "USDT", "BNB" });

            if (!isSuccessGetAllCurrencies || currencies == null)
            {
                NotificationResult.NotificationMessages.Add($"Ошибка получения валют. {messageGetAllCurrencies}");
                _logger.LogError($"Ошибка получения валют. {messageGetAllCurrencies}");
                return false;
            }

            List<CurrencyInfo> currenciesInfo = new(currencies.Select(x => new CurrencyInfo(x.Asset, false, false)));

            if (!currenciesInfo.Any())
            {
                NotificationResult.NotificationMessages.Add($"Нет монет для продажи.");
                _logger.LogTrace($"Нет монет для продажи.");
                return true;
            }

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

            NotificationResult.SuccessCoins.AddRange(currenciesInfo.Where(x => x.IsSell).Select(x => x.Asset).ToList());

            _logger.LogTrace("Продажа монет выполнена");

            #endregion

            #region Перевод монет с маленьким балансом в BNB

            _logger.LogTrace("Начинаем перевод монет с маленьким балансом в BNB");

            // перевод мелких монет в BNB
            (bool isSuccessTransferDust, string messageTransferDust) = await _binanceApiService.TransferDustAsync(currenciesInfo.Where(x => x.IsDust).Select(x => x.Asset).ToList(), settings: settings);

            if (isSuccessTransferDust)
            {
                NotificationResult.NotificationMessages.Add("Перевод монет с маленьким балансом в BNB закончен.");
                _logger.LogTrace("Перевод монет с маленьким балансом в BNB закончен");
            }
            else
            {
                NotificationResult.NotificationMessages.Add($"Перевод монет с маленьким балансом в BNB неудачен. {messageTransferDust}");
                _logger.LogTrace("Перевод монет с маленьким балансом в BNB неудачен");
            }

            #endregion

            (bool isSuccessSellBNB, bool isSellCoinBNB, bool isDustSellBNB) = await SellCoinAsync("BNB", exchangeInfo, settings: settings);

            if (isSuccessSellBNB && isSellCoinBNB)
            {
                NotificationResult.SuccessCoins.Add("BNB");
            }

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
            (bool isSuccessCurrency, string messageCurrency, AssetsInfo currencyInfo) = await _binanceApiService.GetСurrencyAsync(exchangeInfo, currencyAsset, settings: settings);

            if (!isSuccessCurrency || currencyInfo == default || string.IsNullOrEmpty(currencyInfo.FromAsset) || string.IsNullOrEmpty(currencyInfo.ToAsset))
            {
                NotificationResult.NotificationMessages.Add($"Продажа {currencyAsset}: неудачное получение валюты. {messageCurrency}");
                _logger.LogTrace($"Продажа {currencyAsset}: неудачное получение валюты. {messageCurrency}");
                return default;
            }

            if (!currencyInfo.IsDust)
            {
                _logger.LogTrace($"Продажа {currencyAsset}: выполним продажу.");

                (bool isSuccessSell, string messageSell) = await _binanceApiService.SellCoinAsync(currencyInfo.Quantity, currencyInfo.FromAsset, currencyInfo.ToAsset, settings: settings);

                _logger.LogTrace($"Продажа {currencyAsset}:{(isSuccessSell ? "" : " не")} выполнилась продажа по {currencyInfo.ToAsset}. {messageSell}");

                if (!isSuccessSell)
                {
                    NotificationResult.NotificationMessages.Add($"Продажа {currencyAsset}:{(isSuccessSell ? "" : " не")} выполнилась продажа по {currencyInfo.ToAsset}. {messageSell}");
                }

                return (true, isSuccessSell, false);
            }

            return (true, false, true);
        }

        #endregion

        #region Отправка уведолмения в телеграме

        private Task SendTelegramMessageAsync(SettingsInfo settings)
        {
            string message = NotificationResult.GetMessage();

            _logger.LogTrace($"Сообщение для администратора:{Environment.NewLine}{message}");
            if (!settings.IsNotification)
            {
                _logger.LogTrace("Уведомления отключены.");
                return Task.CompletedTask;
            }

            if (!settings.AdminsChatId.Any())
            {
                _logger.LogTrace("Администраторов не найдено для отправки уведомления.");
                return Task.CompletedTask;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                ITelegramMessageQueueRepository telegramMessageQueueRepository =
                    scope.ServiceProvider
                        .GetRequiredService<ITelegramMessageQueueRepository>();

                foreach (var adminChatId in settings.AdminsChatId)
                {
                    telegramMessageQueueRepository.Create(new TelegramMessageQueue()
                    {
                        ChatId = adminChatId,
                        Message = message
                    });

                    _logger.LogTrace($"Отправлено уведомление пользователю {adminChatId}: {message}.");
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}