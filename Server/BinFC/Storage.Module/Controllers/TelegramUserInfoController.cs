using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramUserInfoController : BaseController
    {
        private readonly ITelegramUserInfoRepository _telegramUserInfoRepository;
        private readonly ILogger<TelegramUserInfoController> _logger;
        public TelegramUserInfoController(ITelegramUserInfoRepository telegramUserInfoRepository, ILogger<TelegramUserInfoController> logger)
        {
            _telegramUserInfoRepository = telegramUserInfoRepository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<TelegramUserInfo> Get()
        {
            return _telegramUserInfoRepository.Get();
        }
    }
}