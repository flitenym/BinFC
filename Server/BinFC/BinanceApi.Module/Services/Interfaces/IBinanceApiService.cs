using Binance.Net.Objects.Models.Spot;
using Storage.Module.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using BinanceApi.Module.Classes;

namespace BinanceApi.Module.Services.Interfaces
{
    public interface IBinanceApiService
    {
        public Task<(bool IsSuccess, BinanceExchangeInfo ExchangeInfo)> GetExchangeInfoAsync(SettingsInfo settings);
        public Task<(bool IsSuccess, List<BinanceBalance> Currencies)> GetBinanceCurrenciesAsync(SettingsInfo settings, List<string> except);
        public Task<bool> TransferFuturesToSpotUSDTAsync(SettingsInfo settings);
        public Task<(bool IsSuccess, AssetsInfo Currency)> GetСurrencyAsync(BinanceExchangeInfo exchangeInfo, string asset, SettingsInfo settings);
        public Task<bool> SellCoinAsync(decimal quantity, string fromAsset, string toAsset, SettingsInfo settings);
        public Task<bool> TransferDustAsync(List<string> assets, SettingsInfo settings);
        public Task<(bool IsSuccess, string Message, IEnumerable<BinanceUserAsset> Currencies)> GetCoinsAsync(IEnumerable<string> assets, SettingsInfo settings);
        public Task<(bool IsSuccess, string Message)> WithdrawalPlacedAsync(string asset, decimal amount, string address, string network, SettingsInfo settings);
    }
}