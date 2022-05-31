using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.DTO;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.Services.Interfaces;
using Storage.Module.StaticClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ISpotDataRepository _spotDataRepository;
        private readonly IFuturesDataRepository _futuresDataRepository;
        private readonly ISpotScaleRepository _spotScaleRepository;
        private readonly IFuturesScaleRepository _futuresScaleRepository;
        private readonly ISettingsRepository _settingsRepository;

        private readonly ILogger<PaymentService> _logger;
        public PaymentService(
            ISpotDataRepository spotDataRepository,
            IFuturesDataRepository futuresDataRepository,
            ISpotScaleRepository spotScaleRepository,
            IFuturesScaleRepository futuresScaleRepository,
            ISettingsRepository settingsRepository,
            ILogger<PaymentService> logger
            )
        {
            _spotDataRepository = spotDataRepository;
            _futuresDataRepository = futuresDataRepository;
            _spotScaleRepository = spotScaleRepository;
            _futuresScaleRepository = futuresScaleRepository;
            _settingsRepository = settingsRepository;

            _logger = logger;
        }

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

            if(!scale.Any())
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
    }
}