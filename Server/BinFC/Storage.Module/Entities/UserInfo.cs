using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Module.Entities
{
    public class UserInfo
    {
        [Key]
        public long Id { get; set; }
        public long? ChatId { get; set; }
        /// <summary>
        /// Реферальный ИД (указывается Администратором)
        /// </summary>
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string UserNickName { get; set; }
        public string UserEmail { get; set; }
        public string TrcAddress { get; set; }
        public string BepAddress { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public long? UniqueId { get; set; }


        [ForeignKey("UniqueId")]
        public Unique Unique { get; set; }
    }
}