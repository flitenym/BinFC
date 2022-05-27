using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Module.Entities
{
    public class PayHistory
    {
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// Отправленная сумма
        /// </summary>
        public decimal SendedSum { get; set; }
        /// <summary>
        /// Время отправки
        /// </summary>
        public DateTime SendedTime { get; set; }
        /// <summary>
        /// Номер оплаты (итерация)
        /// </summary>
        public int NumberPay { get; set; }

        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public UserInfo User { get; set; }
    }
}