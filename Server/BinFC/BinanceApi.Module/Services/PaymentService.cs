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

        #region Получение данных для оплаты

        public async Task<(IEnumerable<PaymentDTO> PaymentInfo, string Message)> CalculatePaymentInfoAsync()
        {
            IEnumerable<SpotData> spotData = _spotDataRepository.GetLastData();
            IEnumerable<SpotScale> spotScale = _spotScaleRepository.Get();
            (IEnumerable<PaymentDTO> spotPaymentInfo, string spotMessage) = await GetCalculatedResultAsync("SPOT", SettingsKeys.SpotPercent, spotData, spotScale);

            IEnumerable<FuturesData> futuresData = _futuresDataRepository.GetLastData();
            IEnumerable<FuturesScale> futuresScale = _futuresScaleRepository.Get();
            (IEnumerable<PaymentDTO> futuresPaymentInfo, string futuresMessage) = await GetCalculatedResultAsync("Futures", SettingsKeys.FuturesPercent, futuresData, futuresScale);

            if (!string.IsNullOrEmpty(futuresMessage) || !string.IsNullOrEmpty(spotMessage))
            {
                return (default, string.Join(Environment.NewLine, spotMessage, futuresMessage).Trim());
            }

            return (ConcatPaymentInfo(spotPaymentInfo, futuresPaymentInfo), default);
        }

        /// <summary>
        /// Вычисление сколько нужно выплатить пользователям.
        /// </summary>
        public async Task<(IEnumerable<PaymentDTO> PaymentInfo, string Message)> GetCalculatedResultAsync(
            string commandName,
            string settingsPercentKey,
            IEnumerable<Data> data,
            IEnumerable<Scale> scale)
        {
            List<PaymentDTO> result = new();

            if (!scale.Any())
            {
                return (result, $"{commandName}. Необходимо укахать линейку.");
            }

            (bool isSuccessGetPercent, long settingsPercent) = await _settingsRepository.GetSettingsByKeyAsync<long>(settingsPercentKey, false);

            if (!isSuccessGetPercent)
            {
                return (result, $"{commandName}. Нет данных по проценту в настройках.");
            }

            if (!data.Any())
            {
                return (result, $"{commandName}. Нет данных для получения информации оплаты.");
            }

            List<long> users = data.Select(x => x.UserId).Distinct().ToList();
            List<DateTime> dates = data.Select(x => x.LoadingDate).Distinct().OrderByDescending(x => x).ToList();

            if (!dates.Any())
            {
                return (result, $"{commandName}. Необходимо импортировать 2 разных эксель файла.");
            }

            DateTime lastDate = dates.FirstOrDefault();
            DateTime firstDate = dates.LastOrDefault();

            if (lastDate == firstDate)
            {
                return (result, $"{commandName}. Необходимо импортировать 2 разных эксель файла.");
            }

            // после предварительных проверок получим первичные данные для подсчета

            foreach (var user in users)
            {
                PaymentDTO payment = new PaymentDTO();

                var lastData = data.FirstOrDefault(x => x.UserId == user && x.LoadingDate == lastDate);
                var firstData = data.FirstOrDefault(x => x.UserId == user && x.LoadingDate == firstDate);

                UserInfo userInfo = lastData?.User ?? firstData?.User;

                // сколько заработал учитывая процент из настроек
                double earned = (double)Math.Abs((lastData?.AgentEarnUsdt ?? 0) - (firstData?.AgentEarnUsdt ?? 0)) / settingsPercent * 100.0;

                // ищем нужный процент
                payment.Usdt = (decimal)(earned * (scale.LastOrDefault(x => x.FromValue <= earned && x.UniqueId == userInfo.UniqueId).Percent) / 100.0);

                payment.BepAddress = userInfo.BepAddress;
                payment.TrcAddress = userInfo.TrcAddress;
                payment.UserName = userInfo.UserName;
                payment.UserId = userInfo.UserId.Value;

                result.Add(payment);
            }

            return (result, null);
        }

        /// <summary>
        /// Итоговое соединение информации по выплатам
        /// </summary>
        public IEnumerable<PaymentDTO> ConcatPaymentInfo(IEnumerable<PaymentDTO> spotPaymentInfo, IEnumerable<PaymentDTO> futuresPaymentInfo)
        {
            List<PaymentDTO> result = new();

            List<long> users = spotPaymentInfo.Concat(futuresPaymentInfo).Select(x => x.UserId).Distinct().ToList();

            foreach (long user in users)
            {
                var spotInfo = spotPaymentInfo.FirstOrDefault(x => x.UserId == user);
                var futuresInfo = futuresPaymentInfo.FirstOrDefault(x => x.UserId == user);

                decimal resultUsdt = (spotInfo?.Usdt ?? 0) + (futuresInfo?.Usdt ?? 0);
                if (resultUsdt < 10)
                {
                    continue;
                }

                result.Add(
                    new PaymentDTO()
                    {
                        UserId = user,
                        Usdt = resultUsdt,
                        BepAddress = spotInfo?.BepAddress ?? futuresInfo?.BepAddress,
                        TrcAddress = spotInfo?.TrcAddress ?? futuresInfo?.TrcAddress,
                        UserName = spotInfo?.UserName ?? futuresInfo?.UserName,
                    }
                );
            }

            return result;
        }

        #endregion

        #region Оплата

        public async Task<(bool IsSuccess, string Message)> BinancePayAsync(IEnumerable<PaymentDTO> paymentsInfo)
        {
            if (!paymentsInfo.Any())
            {
                return (false, "Отправлена пустая сущность.");
            }

            SettingsInfo settings = await _settingsRepository.GetSettingsAsync();

            (bool isSuccessGetCoins, string messageGetCoins, IEnumerable<BinanceUserAsset> currencies) =
                await _binanceApiService.GetCoinsAsync(new List<string>() { "USDT", "BUSD" }, settings);

            if (!isSuccessGetCoins)
            {
                return (false, messageGetCoins);
            }

            foreach (var currency in currencies)
            {
                if (currency == null)
                {
                    return (false, "Не найдены доступные монеты для отправки.");
                }
            }

            BinanceNetwork usdtNetwork = currencies.FirstOrDefault(x => x.Asset == "USDT")?.NetworkList.FirstOrDefault(x => x.Network == "TRX");
            BinanceNetwork busdNetwork = currencies.FirstOrDefault(x => x.Asset == "BUSD")?.NetworkList.FirstOrDefault(x => x.Network == "BSC");

            if (usdtNetwork == null && busdNetwork == null)
            {
                return (false, $"Не удалось определить сеть для вывода.");
            }

            if (!(usdtNetwork?.WithdrawEnabled ?? false))
            {
                return (false, $"Для {usdtNetwork?.Network} запрещен вывод");
            }

            if (!(busdNetwork?.WithdrawEnabled ?? false))
            {
                return (false, $"Для {busdNetwork?.Network} запрещен вывод");
            }

            int numberPay = await _payHistoryRepository.GetLastNumberPayAsync();

            NumberFormatInfo setPrecision = new NumberFormatInfo() { NumberDecimalDigits = 6, NumberGroupSeparator = "", NumberDecimalSeparator = "." };

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
                    builder.AppendLine($"Не удалось определить кошелек у пользователя {paymentInfo.UserId}");
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
                        await _payHistoryRepository.CreateAsync(
                            new PayHistory()
                            {
                                SendedSum = paymentInfo.Usdt,
                                NumberPay = numberPay,
                                UserId = paymentInfo.UserId
                            }
                        );
                    }
                    else
                    {
                        builder.AppendLine($"Не удалось отправить пользователю с Id {paymentInfo.UserId}: {messageWithdrawal}");
                    }
                }
                else
                {
                    builder.AppendLine($"Оплата суммы {paymentInfo.Usdt} меньше чем минимальная сумма для перевода {network.WithdrawMin} по {network.Asset}");
                }
            }

            return (true, builder.ToString().Trim());
        }

        #endregion
    }
}