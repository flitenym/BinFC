using Binance.Net.Objects.Models.Spot;
using Storage.Module.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinanceApi.Module.Services.Interfaces
{
    public interface IBinanceApiService
    {
        public Task<(bool IsSuccess, BinanceExchangeInfo ExchangeInfo)> GetExchangeInfoAsync(SettingsInfo settings);
        public Task<(bool IsSuccess, List<BinanceBalance> Currencies)> GetBinanceCurrenciesAsync(SettingsInfo settings, List<string> except);
        public Task<(bool IsSuccess, decimal? Balance)> GetFuturesAccountUsdtBalanceAsync(SettingsInfo settings);
        public Task<bool> TransferFuturesToSpotUSDTAsync(SettingsInfo settings);
        public Task<(bool IsSuccess, (string FromAsset, string ToAsset, decimal Quantity, bool IsDust) Currency)>
            GetСurrencyAsync(BinanceExchangeInfo exchangeInfo, string asset, SettingsInfo settings);
        public Task<(bool IsSuccess, decimal Quantity, bool IsDust, string ToAsset)>
            GetQuantity(BinanceExchangeInfo exchangeInfo, string fromAsset, decimal quantity, SettingsInfo settings);
        public Task<(bool IsSuccess, decimal Quantity, bool IsDust, string ToAsset)>
            GetQuantity(BinanceExchangeInfo exchangeInfo, string fromAsset, string toAsset, decimal quantity, SettingsInfo settings);
        public Task<(bool IsSuccess, BinancePrice Price)> GetPrice(string fromAsset, string toAsset, SettingsInfo settings);
        public Task<bool> SellCoinAsync(decimal quantity, string fromAsset, string toAsset, SettingsInfo settings);
    }
}