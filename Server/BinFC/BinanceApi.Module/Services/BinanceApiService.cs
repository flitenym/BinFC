﻿using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using BinanceApi.Module.Classes;
using BinanceApi.Module.Services.Interfaces;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using Storage.Module.Classes;
using Storage.Module.StaticClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BinanceApi.Module.Services
{
    public class BinanceApiService : IBinanceApiService
    {
        private readonly ILogger<BinanceApiService> _logger;

        public BinanceApiService(ILogger<BinanceApiService> logger)
        {
            _logger = logger;
        }

        private (BinanceClient client, string Message) GetBinanceClient(SettingsInfo settings)
        {
            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                return (null, $"Не указан {nameof(settings.ApiKey)}");
            }

            if (string.IsNullOrEmpty(settings.ApiSecret))
            {
                return (null, $"Не указан {nameof(settings.ApiSecret)}");
            }

            return (new BinanceClient(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(settings.ApiKey, settings.ApiSecret)
            }), null);
        }

        private (bool IsSuccess, string Message) CheckStatus<T>(WebCallResult<T> response)
        {
            if (response.ResponseStatusCode == HttpStatusCode.OK)
            {
                return (true, null);
            }
            else
            {
                _logger.LogError(response.Error.ToString());

                if (response.Error.Code == -2015)
                {
                    return (false, "Отсутствуют ApiKey/ApiSecret или действие не разрешено, включите в настройках Api соответствующие права.");
                }
                else if (response.Error.Code == -2008)
                {
                    return (false, "Неверный ApiKey/ApiSecret.");
                }
                else if (response.Error.Code == 32110)
                {
                    return (false, "Перевод монет с маленьким балансом не выполнен, т.к. можно производить раз в 6 часов.");
                }

                return (false, null);
            }
        }

        #region Перевод Фьюча в Спот

        public async Task<(bool IsSuccess, string Message)> TransferFuturesToSpotUSDTAsync(SettingsInfo settings)
        {
            (bool isAccountUsdtSuccess, string message, decimal? balance) = await GetFuturesAccountUsdtBalanceAsync(settings: settings);

            if (!isAccountUsdtSuccess)
            {
                return (false, message);
            }

            if (balance == 0)
            {
                return (false, $"Во фьючерсе нет {BinanceKeys.USDT}.");
            }

            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message);
            }

            var result = await client.client.GeneralApi.Futures.TransferFuturesAccountAsync(BinanceKeys.USDT, balance.Value, Binance.Net.Enums.FuturesTransferType.FromUsdtFuturesToSpot);

            return CheckStatus(result);
        }

        /// <summary>
        /// Получение баланса по Фьючерсу
        /// </summary>
        private async Task<(bool IsSuccess, string Message, decimal? Balance)> GetFuturesAccountUsdtBalanceAsync(SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message, null);
            }

            var result = await client.client.UsdFuturesApi.Account.GetAccountInfoAsync();

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                return (false, messageStatus, null);
            }

            var futuresUsdtInfo = result.Data.Assets.FirstOrDefault(x => x.Asset == BinanceKeys.USDT);

            if (futuresUsdtInfo == null)
            {
                _logger.LogError($"Ошибка. Не найдена монета {BinanceKeys.USDT} во Фьючерсном кошельке");
                return (false, null, null);
            }

            return (true, null, futuresUsdtInfo.WalletBalance);
        }

        #endregion

        #region Обобщеные методы связанные с аккаунтом

        /// <summary>
        /// Получение системной информации бинанса, включая минимальные значения по валютам
        /// </summary>
        public async Task<(bool IsSuccess, string Message, BinanceExchangeInfo ExchangeInfo)> GetExchangeInfoAsync(SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message, null);
            }

            var result = await client.client.SpotApi.ExchangeData.GetExchangeInfoAsync();

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                return (false, messageStatus, null);
            }

            return (true, null, result.Data);
        }

        /// <summary>
        /// Получение криптовалют из СПОТ аккаунта
        /// </summary>
        /// <param name="except">Список криптовалют, кроме указанных в except.</param>
        public async Task<(bool IsSuccess, string Message, List<BinanceBalance> Currencies)> GetBinanceCurrenciesAsync(SettingsInfo settings, List<string> except)
        {
            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message, default);
            }

            var result = await client.client.SpotApi.Account.GetAccountInfoAsync();

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                return (false, messageStatus, default);
            }

            List<BinanceBalance> currencies;

            if (except == null || except.Count == 0)
            {
                currencies = result.Data.Balances.Where(x => x.Available != 0 && !string.IsNullOrEmpty(x.Asset)).ToList();
            }
            else
            {
                currencies = result.Data.Balances.Where(x => x.Available != 0 && !string.IsNullOrEmpty(x.Asset) && !except.Contains(x.Asset)).ToList();
            }

            return (true, null, currencies);
        }

        #endregion

        #region Получение и продажа валюты

        /// <summary>
        /// Получение информации по криптовалюте
        /// </summary>
        public async Task<(bool IsSuccess, string Message, AssetsInfo Currency)> GetСurrencyAsync(BinanceExchangeInfo exchangeInfo, string asset, SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message, default);
            }

            var result = await client.client.SpotApi.Account.GetAccountInfoAsync();

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                return (false, messageStatus, default);
            }

            var currency = result.Data.Balances.FirstOrDefault(x => x.Available != 0 && x.Asset == asset);

            if (currency != null)
            {
                (bool isSuccessQuantity, string messageQuantity, AssetsInfo assetInfo) = await GetQuantity(exchangeInfo, currency.Asset, currency.Available * 0.98m, settings: settings);

                if (!isSuccessQuantity)
                {
                    return (false, messageQuantity, default);
                }

                _logger.LogTrace($"Для {assetInfo.FromAsset} ({assetInfo.ToAsset}) количество определилось как {assetInfo.Quantity} из {currency.Available}({currency.Available}), {(assetInfo.IsDust ? "ПЫЛЬ" : "НЕ ПЫЛЬ")}.");
                return (true, null, assetInfo);
            }
            else
            {
                _logger.LogInformation($"Не найдена монета {asset}");
                return (true, null, default);
            }
        }

        /// <summary>
        /// Продажа монеты
        /// </summary>
        /// <param name="quantity">Количество перевода</param>
        /// <param name="fromAsset">Из какой валюты</param>
        /// <param name="toAsset">В какую валюту</param>
        public async Task<(bool IsSuccess, string Message)> SellCoinAsync(decimal quantity, string fromAsset, string toAsset, SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message);
            }

            var result = await client.client.SpotApi.Trading.PlaceOrderAsync($"{fromAsset}{toAsset}", Binance.Net.Enums.OrderSide.Sell, Binance.Net.Enums.SpotOrderType.Market, quoteQuantity: quantity);

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                _logger.LogError($"Ошибка продажи валюты {fromAsset}{toAsset}. {result.Error}" + result.Error.ToString());
                return (false, messageStatus);
            }

            return (true, null);
        }

        #endregion

        #region Продажа пыли

        public async Task<(bool IsSuccess, string Message)> TransferDustAsync(List<string> assets, SettingsInfo settings)
        {
            if (!assets.Any())
            {
                _logger.LogTrace($"Нет монет с маленьким балансом.");
                return (true, null);
            }
            else
            {
                _logger.LogTrace($"Монеты с маленьким балансом: {string.Join(", ", assets)}");
            }

            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message);
            }

            var result = await client.client.SpotApi.Account.DustTransferAsync(assets);

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                return (false, messageStatus);
            }

            return (true, null);
        }

        #endregion

        #region Получение доп. данных при продаже валюты

        private async Task<(bool IsSuccess, string Message, AssetsInfo AssetInfo)> GetQuantity(BinanceExchangeInfo exchangeInfo, string fromAsset, decimal quantity, SettingsInfo settings)
        {
            (bool isSuccessGetQuantityUSDT, string messageGetQuantityUSDT, AssetsInfo usdtAssetInfo) = await GetQuantity(exchangeInfo, fromAsset, BinanceKeys.USDT, quantity, settings: settings);

            if (isSuccessGetQuantityUSDT && !string.IsNullOrEmpty(usdtAssetInfo.ToAsset))
            {
                return (true, null, usdtAssetInfo);
            }
            else if (string.IsNullOrEmpty(usdtAssetInfo.ToAsset))
            {
                (bool isSuccessGetQuantityAnother, string messageGetQuantityAnother, AssetsInfo anotherAssetInfo) = await GetQuantity(exchangeInfo, fromAsset, null, quantity, settings: settings);

                if (!isSuccessGetQuantityAnother)
                {
                    return (false, messageGetQuantityAnother, null);
                }

                if (isSuccessGetQuantityAnother && !string.IsNullOrEmpty(anotherAssetInfo.ToAsset))
                {
                    return (true, null, anotherAssetInfo);
                }
            }

            return (false, messageGetQuantityUSDT, null);
        }

        /// <summary>
        /// Получение количества монет, которое удовлетворяет правилам выставления ордера на маркет
        /// </summary>
        /// <param name="exchangeInfo">Системная информация по валютам</param>
        /// <param name="asset">Валюта</param>
        /// <param name="quantity">Количество для продажи</param>
        /// <returns></returns>
        private async Task<(bool IsSuccess, string Message, AssetsInfo AssetInfo)> GetQuantity(BinanceExchangeInfo exchangeInfo, string fromAsset, string toAsset, decimal quantity, SettingsInfo settings)
        {
            BinanceSymbol symbolInfo;
            if (string.IsNullOrEmpty(toAsset))
            {
                symbolInfo = exchangeInfo?.Symbols?.FirstOrDefault(x => x?.BaseAsset == fromAsset);
            }
            else
            {
                symbolInfo = exchangeInfo?.Symbols?.FirstOrDefault(x => x?.BaseAsset == fromAsset && x?.QuoteAsset == toAsset);
            }

            if (symbolInfo == null)
            {
                _logger.LogTrace($"Не найдены фильтры для {fromAsset}");
                return (false, null, new AssetsInfo(fromAsset, null, default(decimal), false));
            }

            var symbolFilterLotSize = symbolInfo.LotSizeFilter;
            var symbolFilterMinNotional = symbolInfo.MinNotionalFilter;

            (bool isSuccessGetPriceInfo, string messageGetPriceInfo, BinancePrice price) = await GetPrice(fromAsset, symbolInfo.QuoteAsset, settings: settings);

            if (!isSuccessGetPriceInfo)
            {
                _logger.LogTrace($"Не удалось получить цену для {fromAsset}{symbolInfo.QuoteAsset}");
                return (false, messageGetPriceInfo, new AssetsInfo(fromAsset, symbolInfo.QuoteAsset, default(decimal), false));
            }

            // преобразует число, например 304.012334 если stepsize = 0.001 в 304.012
            decimal resultQuantity = Math.Round(quantity * price.Price, BitConverter.GetBytes(decimal.GetBits(symbolFilterLotSize.StepSize / 1.0000000000m)[3])[2], MidpointRounding.ToNegativeInfinity)
                + new decimal(0, 0, 0, false, (byte)symbolInfo.BaseAssetPrecision);

            _logger.LogTrace($"ResultQuantity: {resultQuantity}, StepSize: {symbolFilterLotSize.StepSize}, MinQuantity: {symbolFilterLotSize.MinQuantity}, MaxQuantity: {symbolFilterLotSize.MaxQuantity}");

            if (resultQuantity == 0)
            {
                _logger.LogTrace($"Полученное значение для {fromAsset} = 0");
                return (true, null, new AssetsInfo(fromAsset, symbolInfo.QuoteAsset, resultQuantity, true));
            }

            if (resultQuantity >= symbolFilterLotSize.MinQuantity && resultQuantity <= symbolFilterLotSize.MaxQuantity)
            {
                _logger.LogTrace("Проверку на LOT_SIZE прошло");

                if (!isSuccessGetPriceInfo)
                {
                    return default;
                }

                _logger.LogTrace($"ResultQuantity: {resultQuantity}, Price: {price.Price}, MinNotional: {symbolFilterMinNotional.MinNotional}");

                if (resultQuantity * 1.05m < symbolFilterMinNotional.MinNotional)
                {
                    _logger.LogTrace("Проверку на MIN_NOTIONAL не прошло");
                    return (true, null, new AssetsInfo(fromAsset, symbolInfo.QuoteAsset, resultQuantity, true));
                }
                else
                {
                    _logger.LogTrace("Проверку на MIN_NOTIONAL прошло");
                    return (true, null, new AssetsInfo(fromAsset, symbolInfo.QuoteAsset, resultQuantity, false));
                }
            }

            return default;
        }

        /// <summary>
        /// Получение цены между валютами
        /// </summary>
        /// <param name="fromAsset">Первая валюта</param>
        /// <param name="toAsset">Вторая валюта</param>
        private async Task<(bool IsSuccess, string Message, BinancePrice Price)> GetPrice(string fromAsset, string toAsset, SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message, default);
            }

            var result = await client.client.SpotApi.ExchangeData.GetPriceAsync($"{fromAsset}{toAsset}");

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                return (false, messageStatus, default);
            }

            return (true, null, result.Data);
        }

        #endregion

        #region Методы для оплаты

        public async Task<(bool IsSuccess, string Message, IEnumerable<BinanceUserAsset> Currencies)> GetCoinsAsync(IEnumerable<string> assets, SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message, default);
            }

            var result = await client.client.SpotApi.Account.GetUserAssetsAsync();

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                return (false, messageStatus, default);
            }

            return (true, null, result.Data.Where(x => assets.Contains(x.Asset)));
        }

        public async Task<(bool IsSuccess, string Message)> WithdrawalPlacedAsync(string asset, decimal amount, string address, string network, SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            if (!string.IsNullOrEmpty(client.Message))
            {
                return (false, client.Message);
            }

            var result = await client.client.SpotApi.Account.WithdrawAsync(asset: asset, address: address, quantity: amount, network: network);

            (bool isSuccessStatus, string messageStatus) = CheckStatus(result);

            if (!isSuccessStatus)
            {
                return (false, messageStatus);
            }

            return (true, null);
        }

        #endregion
    }
}