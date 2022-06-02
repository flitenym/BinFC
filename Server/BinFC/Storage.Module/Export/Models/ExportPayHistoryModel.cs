using Storage.Module.Entities;
using System;

namespace Storage.Module.Export.Models
{
    public class ExportPayHistoryModel
    {
        public ExportPayHistoryModel(PayHistory payHistory)
        {
            UserId = payHistory.User.UserId;
            UserName = payHistory.User.UserName;
            SendedSum = payHistory.SendedSum;
            SendedTime = payHistory.SendedTime;
            NumberPay = payHistory.NumberPay;
        }

        public long? UserId { get; set; }
        public string UserName { get; set; }
        public decimal SendedSum { get; set; }
        /// <summary>
        /// Время отправки
        /// </summary>
        public DateTime SendedTime { get; set; }
        /// <summary>
        /// Номер оплаты (итерация)
        /// </summary>
        public int NumberPay { get; set; }
    }
}