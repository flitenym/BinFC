﻿using System.ComponentModel.DataAnnotations;
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
        public string UserEmail { get; set; }
        public string TrcAddress { get; set; }
        public string BepAddress { get; set; }
        public bool IsAdmin { get; set; } = false;
        public long UniqueId { get; set; } = DefaultValues.UniqueId;


        [ForeignKey("UniqueId")]
        public Unique Unique { get; set; }
    }
}