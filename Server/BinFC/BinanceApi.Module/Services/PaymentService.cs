using BinanceApi.Module.Services.Interfaces;
using Microsoft.Extensions.Logging;
using BinanceApi.Module.Controllers.DTO;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.StaticClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net.Objects.Models.Spot;
using System.Globalization;
using Storage.Module.Classes;
using System.Text;
using BinanceApi.Module.Localization;

namespace BinanceApi.Module.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBinanceApiService _binanceApiService;
        private readonly ISpotDataRepository _spotDataRepository;
        private readonly IFuturesDataRepository _futuresDataRepository;
        private readonly ISpotScaleRepository _spotScaleRepository;
        private readonly IFuturesScaleRepository _futuresScaleRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IPayHistoryRepository _payHistoryRepository;

        private readonly ILogger<PaymentService> _logger;
        public PaymentService(
            IBinanceApiService binanceApiService,
            ISpotDataRepository spotDataRepository,
            IFuturesDataRepository futuresDataRepository,
            ISpotScaleRepository spotScaleRepository,
            IFuturesScaleRepository futuresScaleRepository,
            ISettingsRepository settingsRepository,
            IPayHistoryRepository payHistoryRepository,
            ILogger<PaymentService> logger
            )
        {
            _binanceApiService = binanceApiService;
            _spotDataRepository = spotDataRepository;
            _futuresDataRepository = futuresDataRepository;
            _spotScaleRepository = spotScaleRepository;
            _futuresScaleRepository = futuresScaleRepository;
            _settingsRepository = settingsRepository;
            _payHistoryRepository = payHistoryRepository;

            _logger = logger;
        }

        #region Общее

        public async Task<(bool IsSuccess, string Message, BinanceUserAsset usdt, BinanceUserAsset busd)> GetCurrenciesAsync(SettingsInfo settings)
        {
            (bool isSuccessGetCoins, string messageGetCoins, IEnumerable<BinanceUserAsset> currencies) =
                await _binanceApiService.GetCoinsAsync(new List<string>() { BinanceKeys.USDT, BinanceKeys.BUSD }, settings);

            if (!isSuccessGetCoins)
            {
                return (false, messageGetCoins, default, default);
            }

            return (true, default, currencies.FirstOrDefault(x => x.Asset == BinanceKeys.USDT), currencies.FirstOrDefault(x => x.Asset == BinanceKeys.BUSD));
        }

        /// <summary>
        /// Получение информации по сетям для отправки.
        /// </summary>
        public (bool IsSuccess, string Message, BinanceNetwork UsdtNetwork, BinanceNetwork BusdNetwork) GetNetworks(BinanceUserAsset usdt, BinanceUserAsset busd)
        {
            BinanceNetwork usdtNetwork = usdt?.NetworkList.FirstOrDefault(x => x.Network == BinanceKeys.TRX);
            BinanceNetwork busdNetwork = busd?.NetworkList.FirstOrDefault(x => x.Network == BinanceKeys.BSC);

            if (usdtNetwork == null && busdNetwork == null)
            {
                return (false, BinanceApiLoc.NetworksNotFound, default, default);
            }

            if (!(usdtNetwork?.WithdrawEnabled ?? false))
            {
                return (false, string.Format(BinanceApiLoc.NetworkWithdrawDisabled, usdtNetwork?.Network), default, default);
            }

            if (!(busdNetwork?.WithdrawEnabled ?? false))
            {
                return (false, string.Format(BinanceApiLoc.NetworkWithdrawDisabled, busdNetwork?.Network), default, default);
            }

            return (true, default, usdtNetwork, busdNetwork);
        }

        #endregion

        #region Получение данных для оплаты

        public async Task<(IEnumerable<PaymentDTO> PaymentInfo, bool IsSuccess, string Message)> CalculatePaymentInfoAsync()
        {
            IEnumerable<SpotData> spotData = _spotDataRepository.GetLastData();
            IEnumerable<SpotScale> spotScale = _spotScaleRepository.GetSorted();
            (IEnumerable<PaymentDTO> spotPaymentInfo, bool spotIsSuccess, string spotMessage) = await GetCalculatedResultAsync(BinanceKeys.SPOT, SettingsKeys.SpotPercent, spotData, spotScale);

            IEnumerable<FuturesData> futuresData = _futuresDataRepository.GetLastData();
            IEnumerable<FuturesScale> futuresScale = _futuresScaleRepository.GetSorted();
            (IEnumerable<PaymentDTO> futuresPaymentInfo, bool futuresIsSuccess, string futuresMessage) = await GetCalculatedResultAsync(BinanceKeys.FUTURES, SettingsKeys.FuturesPercent, futuresData, futuresScale);

            if (!spotIsSuccess && !string.IsNullOrEmpty(futuresMessage) || !futuresIsSuccess && !string.IsNullOrEmpty(spotMessage))
            {
                return (default, false, string.Join(Environment.NewLine, spotMessage, futuresMessage).Trim());
            }

            (IEnumerable<PaymentDTO> paymentInfo, string message) = await ConcatPaymentInfoAsync(spotPaymentInfo, futuresPaymentInfo);

            return (paymentInfo, true, message);
        }

        /// <summary>
        /// Вычисление сколько нужно выплатить пользователям.
        /// </summary>
        public async Task<(IEnumerable<PaymentDTO> PaymentInfo, bool IsSuccess, string Message)> GetCalculatedResultAsync(
            string commandName,
            string settingsPercentKey,
            IEnumerable<Data> data,
            IEnumerable<Scale> scales)
        {
            List<PaymentDTO> result = new();

            if (!scales.Any())
            {
                return (result, false, string.Format(BinanceApiLoc.ScalesNotFound, commandName));
            }

            (bool isSuccessGetPercent, long settingsPercent) = await _settingsRepository.GetSettingsByKeyAsync<long>(settingsPercentKey, false);

            if (!isSuccessGetPercent)
            {
                return (result, false, string.Format(BinanceApiLoc.GetSettingsPercent, commandName));
            }

            if (!data.Any())
            {
                return (result, false, string.Format(BinanceApiLoc.DataNotFound, commandName));
            }

            List<long> users = data.Select(x => x.UserId).Distinct().ToList();
            List<DateTime> dates = data.Select(x => x.LoadingDate).Distinct().OrderByDescending(x => x).ToList();

            if (!dates.Any())
            {
                return (result, false, string.Format(BinanceApiLoc.NeedTwoExcelFiles, commandName));
            }

            DateTime lastDate = dates.FirstOrDefault();
            DateTime firstDate = dates.LastOrDefault();

            if (lastDate == firstDate)
            {
                return (result, false, string.Format(BinanceApiLoc.NeedTwoExcelFiles, commandName));
            }

            // после предварительных проверок получим первичные данные для подсчета

            foreach (var user in users)
            {
                var lastData = data.FirstOrDefault(x => x.UserId == user && x.LoadingDate == lastDate);
                var firstData = data.FirstOrDefault(x => x.UserId == user && x.LoadingDate == firstDate);

                if (lastData?.IsPaid == true)
                {
                    lastData = null;
                }
                
                if (firstData?.IsPaid == true)
                {
                    firstData = null;
                }

                if (lastData == null && firstData == null)
                {
                    continue;
                }

                PaymentDTO payment = new PaymentDTO();

                UserInfo userInfo = lastData?.User ?? firstData?.User;

                if (userInfo == null)
                {
                    continue;
                }

                if (!userInfo.IsApproved)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(userInfo.BepAddress) && string.IsNullOrEmpty(userInfo.TrcAddress))
                {
                    continue;
                }

                // сколько заработал учитывая процент из настроек
                double earned = (double)Math.Abs((lastData?.AgentEarnUsdt ?? 0) - (firstData?.AgentEarnUsdt ?? 0)) / settingsPercent * 100.0;

                // найдем удовлетворяющую шкалу для пользователя.
                var scale = scales.LastOrDefault(x => x.FromValue <= earned && x.UniqueId == userInfo.UniqueId);

                if (scale == null)
                {
                    return (default, false, string.Format(BinanceApiLoc.NeedScalesForUser, commandName, userInfo.UserId));
                }

                // ищем нужный процент
                payment.Usdt = (decimal)(earned * scale.Percent / 100.0);

                payment.BepAddress = userInfo.BepAddress;
                payment.TrcAddress = userInfo.TrcAddress;
                payment.UserName = userInfo.UserName;
                payment.UserId = userInfo.UserId.Value;

                result.Add(payment);
            }

            return (result, true, null);
        }

        /// <summary>
        /// Итоговое соединение информации по выплатам
        /// </summary>
        public async Task<(IEnumerable<PaymentDTO> PaymentInfo, string Message)> ConcatPaymentInfoAsync(IEnumerable<PaymentDTO> spotPaymentInfo, IEnumerable<PaymentDTO> futuresPaymentInfo)
        {
            List<PaymentDTO> result = new();

            List<long> users = spotPaymentInfo.Concat(futuresPaymentInfo).Select(x => x.UserId).Distinct().ToList();

            SettingsInfo settings = await _settingsRepository.GetSettingsAsync();

            (bool isSuccessGetCurrencies, string messageGetCurrencies, BinanceUserAsset usdt, BinanceUserAsset busd) = await GetCurrenciesAsync(settings);

            if (!isSuccessGetCurrencies)
            {
                return (default, messageGetCurrencies);
            }

            (bool isSuccessGetNetworks, string messageGetNetworks, BinanceNetwork usdtNetwork, BinanceNetwork busdNetwork) = GetNetworks(usdt, busd);

            if (!isSuccessGetNetworks)
            {
                return (default, messageGetNetworks);
            }

            foreach (long user in users)
            {
                BinanceNetwork network = null;

                var spotInfo = spotPaymentInfo.FirstOrDefault(x => x.UserId == user);
                var futuresInfo = futuresPaymentInfo.FirstOrDefault(x => x.UserId == user);

                decimal resultUsdt = (spotInfo?.Usdt ?? 0) + (futuresInfo?.Usdt ?? 0);
                string bepAddress = spotInfo?.BepAddress ?? futuresInfo?.BepAddress;
                string trcAddress = spotInfo?.TrcAddress ?? futuresInfo?.TrcAddress;

                if (!string.IsNullOrEmpty(trcAddress))
                {
                    network = usdtNetwork;
                }
                else if (!string.IsNullOrEmpty(bepAddress))
                {
                    network = busdNetwork;
                }

                if (network == null)
                {
                    continue;
                }

                if (resultUsdt < network.WithdrawMin)
                {
                    continue;
                }

                result.Add(
                    new PaymentDTO()
                    {
                        UserId = user,
                        Usdt = resultUsdt,
                        BepAddress = bepAddress,
                        TrcAddress = trcAddress,
                        UserName = spotInfo?.UserName ?? futuresInfo?.UserName,
                    }
                );
            }

            return (result, default);
        }

        #endregion

        #region Оплата

        public async Task<(bool IsSuccess, string Message)> BinancePayAsync(IEnumerable<PaymentDTO> paymentsInfo)
        {
            if (!paymentsInfo.Any())
            {
                return (false, BinanceApiLoc.Empty);
            }

            SettingsInfo settings = await _settingsRepository.GetSettingsAsync();

            (bool isSuccessGetCurrencies, string messageGetCurrencies, BinanceUserAsset usdt, BinanceUserAsset busd) = await GetCurrenciesAsync(settings);

            if (!isSuccessGetCurrencies)
            {
                return (false, messageGetCurrencies);
            }

            var trcBalance = paymentsInfo.Where(x => !string.IsNullOrEmpty(x.TrcAddress)).Sum(x => x.Usdt);

            if ((usdt?.Available ?? 0) < trcBalance)
            {
                return (false, string.Format(BinanceApiLoc.BalanceLess, BinanceKeys.USDT, (usdt?.Available ?? 0), trcBalance));
            }

            var bepBalance = paymentsInfo.Where(x => !string.IsNullOrEmpty(x.BepAddress)).Sum(x => x.Usdt);

            if ((busd?.Available ?? 0) < bepBalance)
            {
                return (false, string.Format(BinanceApiLoc.BalanceLess, BinanceKeys.BUSD, (busd?.Available ?? 0), bepBalance));
            }

            (bool isSuccessGetNetworks, string messageGetNetworks, BinanceNetwork usdtNetwork, BinanceNetwork busdNetwork) = GetNetworks(usdt, busd);

            if (!isSuccessGetNetworks)
            {
                return (false, messageGetNetworks);
            }


            int numberPay = await _payHistoryRepository.GetLastNumberPayAsync();

            NumberFormatInfo setPrecision = new NumberFormatInfo() { NumberDecimalDigits = 6, NumberGroupSeparator = "", NumberDecimalSeparator = "." };

            bool isHaveOnePay = false;

            StringBuilder builder = new StringBuilder();

            foreach (PaymentDTO paymentInfo in paymentsInfo)
            {
                // подсчитаем корректную сумму для отправки
                paymentInfo.Usdt = decimal.Parse(paymentInfo.Usdt.ToString("N", setPrecision).Replace('.', ','));

                // выберем кошелек, по которому будет отправка
                BinanceNetwork network = null;

                if (!string.IsNullOrEmpty(paymentInfo.TrcAddress))
                {
                    network = usdtNetwork;
                }
                else if (!string.IsNullOrEmpty(paymentInfo.BepAddress))
                {
                    network = busdNetwork;
                }

                if (network == null)
                {
                    builder.AppendLine(string.Format(BinanceApiLoc.PursesEmpty, paymentInfo.UserId));
                    continue;
                }

                // если для отправки больше минимального допустимого, то будем отправлять.
                if (paymentInfo.Usdt >= network.WithdrawMin)
                {
                    (bool isSuccessWithdrawal, string messageWithdrawal) =
                        await _binanceApiService.WithdrawalPlacedAsync(
                            network.Asset,
                            paymentInfo.Usdt,
                            paymentInfo.TrcAddress ?? paymentInfo.BepAddress,
                            network.Network,
                            settings);

                    if (isSuccessWithdrawal)
                    {
                        // выполним без сохранения, а в _payHistoryRepository.CreateAsync выполним сохранение.

                        // обновим, что оплата произошла у данного userId, чтобы в последующем он уже не выбирался, т.к. ему уже оплатили.
                        await _spotDataRepository.UpdateIsPaidByUserIdAsync(paymentInfo.UserId);
                        await _futuresDataRepository.UpdateIsPaidByUserIdAsync(paymentInfo.UserId);

                        // добавим в историю о выплатах
                        var saveMessage = await _payHistoryRepository.CreateAsync(
                            new PayHistory()
                            {
                                SendedSum = paymentInfo.Usdt,
                                NumberPay = numberPay,
                                UserId = paymentInfo.UserId
                            },
                            paymentInfo.UserId
                        );

                        if (!string.IsNullOrEmpty(saveMessage))
                        {
                            builder.AppendLine(string.Format(BinanceApiLoc.CanNotCreatePayHistory, paymentInfo.UserId, saveMessage));
                        }
                    }
                    else
                    {
                        builder.AppendLine(string.Format(BinanceApiLoc.CanNotSendMoney, paymentInfo.UserId, messageWithdrawal));
                    }
                }
                else
                {
                    builder.AppendLine(string.Format(BinanceApiLoc.WithdrawMinLessPay, paymentInfo.Usdt, network.WithdrawMin, network.Asset));
                }
            }

            string haveOnePay = isHaveOnePay ? BinanceApiLoc.PaySuccess : BinanceApiLoc.PayUnsuccess;

            return (true, $"{haveOnePay}{Environment.NewLine}{builder.ToString().Trim()}");
        }

        #endregion

        #region Получение информации по текущему курсу

        public async Task<string> GetBinanceBalanceAsync()
        {
            SettingsInfo settings = await _settingsRepository.GetSettingsAsync();

            (bool isSuccessGetCurrencies, string messageGetCurrencies, BinanceUserAsset usdt, BinanceUserAsset busd) = await GetCurrenciesAsync(settings);

            if (!isSuccessGetCurrencies)
            {
                return messageGetCurrencies;
            }

            return $"{BinanceApiLoc.Balance}:" + Environment.NewLine +
                string.Join(Environment.NewLine, $"{usdt.Asset}: {usdt.Available}", $"{busd.Asset}: {busd.Available}").Trim();
        }

        #endregion
    }
}