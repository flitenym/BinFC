﻿using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using BinanceApi.Module.Services.Interfaces;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using Storage.Module.Classes;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BinanceApi.Module.Services
{
    public class BinanceApiService : IBinanceApiService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger<BinanceApiService> _logger;

        public const HttpStatusCode SuccessCode = HttpStatusCode.OK;
        public const int DustTransferSixHours = 32110;
        public const string USDT = "USDT";

        public BinanceApiService(ISettingsRepository settingsRepository, ILogger<BinanceApiService> logger)
        {
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        private BinanceClient GetBinanceClient(SettingsInfo settings)
        {
            return new BinanceClient(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(settings.ApiKey, settings.ApiSecret)
            });
        }

        /// <summary>
        /// Получение системной информации бинанса, включая минимальные значения по валютам
        /// </summary>
        public async Task<(bool IsSuccess, BinanceExchangeInfo ExchangeInfo)> GetExchangeInfoAsync(SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            var result = await client.SpotApi.ExchangeData.GetExchangeInfoAsync();

            if (result.ResponseStatusCode != SuccessCode)
            {
                _logger.LogError(result.Error.ToString());
                return (false, null);
            }

            return (true, result.Data);
        }

        public async Task<(bool IsSuccess, List<BinanceBalance> Currencies)> GetBinanceCurrenciesAsync(SettingsInfo settings, List<string> except)
        {
            var client = GetBinanceClient(settings);

            var result = await client.SpotApi.Account.GetAccountInfoAsync();

            if (result.ResponseStatusCode != SuccessCode)
            {
                _logger.LogError(result.Error.ToString());
                return (false, null);
            }

            List<BinanceBalance> currencies;

            if (except == null || except.Count == 0)
            {
                currencies = result.Data.Balances.Where(x => x.Available != 0).ToList();
            }
            else
            {
                currencies = result.Data.Balances.Where(x => x.Available != 0 && !except.Contains(x.Asset)).ToList();
            }

            return (true, currencies);
        }

        /// <summary>
        /// Получение баланса по Фьючерсу
        /// </summary>
        public async Task<(bool IsSuccess, decimal? Balance)> GetFuturesAccountUsdtBalanceAsync(SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            var result = await client.UsdFuturesApi.Account.GetAccountInfoAsync();

            if (result.ResponseStatusCode != SuccessCode)
            {
                _logger.LogError(result.Error.ToString());
                return (false, null);
            }

            var futuresUsdtInfo = result.Data.Assets.FirstOrDefault(x => x.Asset == USDT);

            if (futuresUsdtInfo == null)
            {
                _logger.LogError($"Ошибка. Не найдена монета {USDT} во Фьючерсном кошельке");
                return (false, null);
            }

            return (true, futuresUsdtInfo.WalletBalance);
        }

        public async Task<bool> TransferFuturesToSpotUSDTAsync(SettingsInfo settings)
        {
            (bool isAccountUsdtSuccess, decimal? balance) = await GetFuturesAccountUsdtBalanceAsync(settings: settings);

            if (!isAccountUsdtSuccess)
            {
                return false;
            }

            var client = GetBinanceClient(settings);

            var result = await client.GeneralApi.Futures.TransferFuturesAccountAsync(USDT, balance.Value, Binance.Net.Enums.FuturesTransferType.FromUsdtFuturesToSpot);

            if (result.ResponseStatusCode != SuccessCode)
            {
                _logger.LogError(result.Error.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Получение информации по криптовалюте
        /// </summary>
        public async Task<(bool IsSuccess, (string FromAsset, string ToAsset, decimal Quantity, bool IsDust) Currency)>
            GetСurrencyAsync(BinanceExchangeInfo exchangeInfo, string asset, SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            var result = await client.SpotApi.Account.GetAccountInfoAsync();

            if (result.ResponseStatusCode != SuccessCode)
            {
                _logger.LogError(result.Error.ToString());
                return (false, default);
            }

            var currency = result.Data.Balances.FirstOrDefault(x => x.Available != 0 && x.Asset == asset);

            if (currency != null)
            {
                (bool isSuccessQuantity, decimal resultQuantity, bool isDust, string toAsset) = 
                    await GetQuantity(exchangeInfo, currency.Asset, currency.Available * 0.98m, settings: settings);

                if (!isSuccessQuantity)
                {
                    return (false, default);
                }

                _logger.LogTrace($"Для {currency.Asset} ({toAsset}) количество определилось как {resultQuantity} из {currency.Available}({currency.Available}), {(isDust ? "ПЫЛЬ" : "НЕ ПЫЛЬ")}.");
                return (true, (currency.Asset, toAsset, resultQuantity, isDust));
            }
            else
            {
                _logger.LogInformation($"Не найдена монета {asset}");
                return (true, default);
            }
        }

        public async Task<(bool IsSuccess, decimal Quantity, bool IsDust, string ToAsset)> 
            GetQuantity(BinanceExchangeInfo exchangeInfo, string fromAsset, decimal quantity, SettingsInfo settings)
        {
            (bool isSuccessGetQuantityUSDT, decimal resultQuantityUsdt, bool isDustUsdt, string toAssetUsdt) = 
                await GetQuantity(exchangeInfo, fromAsset, USDT, quantity, settings: settings);

            if (isSuccessGetQuantityUSDT && !string.IsNullOrEmpty(toAssetUsdt))
            {
                return (true, resultQuantityUsdt, isDustUsdt, toAssetUsdt);
            }
            else if (string.IsNullOrEmpty(toAssetUsdt))
            {
                (bool isSuccessGetQuantityAnother, decimal resultQuantityAnother, bool isDustAnother, string toAssetAnother) = 
                    await GetQuantity(exchangeInfo, fromAsset, null, quantity, settings: settings);

                if (isSuccessGetQuantityAnother && !string.IsNullOrEmpty(toAssetAnother))
                {
                    return (true, resultQuantityAnother, isDustAnother, toAssetAnother);
                }
            }

            return default;
        }

        /// <summary>
        /// Получение количества монет, которое удовлетворяет правилам выставления ордера на маркет
        /// </summary>
        /// <param name="exchangeInfo">Системная информация по валютам</param>
        /// <param name="asset">Валюта</param>
        /// <param name="quantity">Количество для продажи</param>
        /// <returns></returns>
        public async Task<(bool IsSuccess, decimal Quantity, bool IsDust, string ToAsset)> 
            GetQuantity(BinanceExchangeInfo exchangeInfo, string fromAsset, string toAsset, decimal quantity, SettingsInfo settings)
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
                return (false, default(decimal), false, null);
            }

            var symbolFilterLotSize = symbolInfo.LotSizeFilter;
            var symbolFilterMinNotional = symbolInfo.MinNotionalFilter;

            (bool isSuccessGetPriceInfo, BinancePrice price) = await GetPrice(fromAsset, symbolInfo.QuoteAsset, settings: settings);

            if (!isSuccessGetPriceInfo)
            {
                _logger.LogTrace($"Не удалось получить цену для {fromAsset}{symbolInfo.QuoteAsset}");
                return (false, default(decimal), false, symbolInfo.QuoteAsset);
            }

            // преобразует число, например 304.012334 если stepsize = 0.001 в 304.012
            decimal resultQuantity = Math.Round(quantity * price.Price, BitConverter.GetBytes(decimal.GetBits(symbolFilterLotSize.StepSize / 1.0000000000m)[3])[2], MidpointRounding.ToNegativeInfinity)
                + new decimal(0, 0, 0, false, (byte)symbolInfo.BaseAssetPrecision);

            _logger.LogTrace($"ResultQuantity: {resultQuantity}, StepSize: {symbolFilterLotSize.StepSize}, MinQuantity: {symbolFilterLotSize.MinQuantity}, MaxQuantity: {symbolFilterLotSize.MaxQuantity}");

            if (resultQuantity == 0)
            {
                _logger.LogTrace($"Полученное значение для {fromAsset} = 0");
                return (true, resultQuantity, true, symbolInfo.QuoteAsset);
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
                    return (true, resultQuantity, true, symbolInfo.QuoteAsset);
                }
                else
                {
                    _logger.LogTrace("Проверку на MIN_NOTIONAL прошло");
                    return (true, resultQuantity, false, symbolInfo.QuoteAsset);
                }
            }

            return default;
        }

        /// <summary>
        /// Получение цены между валютами
        /// </summary>
        /// <param name="fromAsset">Первая валюта</param>
        /// <param name="toAsset">Вторая валюта</param>
        public async Task<(bool IsSuccess, BinancePrice Price)> GetPrice(string fromAsset, string toAsset, SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            var result = await client.SpotApi.ExchangeData.GetPriceAsync($"{fromAsset}{toAsset}");

            if (result.ResponseStatusCode != SuccessCode)
            {
                _logger.LogError(result.Error.ToString());
                return (false, null);
            }

            return (true, result.Data);
        }

        /// <summary>
        /// Продажа монеты
        /// </summary>
        /// <param name="quantity">Количество перевода</param>
        /// <param name="fromAsset">Из какой валюты</param>
        /// <param name="toAsset">В какую валюту</param>
        public async Task<bool> SellCoinAsync(decimal quantity, string fromAsset, string toAsset, SettingsInfo settings)
        {
            var client = GetBinanceClient(settings);

            var result = await client.SpotApi.Trading.PlaceOrderAsync($"{fromAsset}{toAsset}", Binance.Net.Enums.OrderSide.Sell, Binance.Net.Enums.SpotOrderType.Market, quoteQuantity: quantity);

            if (result.ResponseStatusCode != SuccessCode)
            {
                _logger.LogError($"Ошибка продажи валюты {fromAsset}{toAsset}. {result.Error}" + result.Error.ToString());
                return false;
            }

            return true;
        }
    }
}