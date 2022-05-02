using Cronos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkerService.Module
{
    [Route("api/[controller]")]
    [ApiController]
    public class CronJobController : BaseController
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger<BinanceSellController> _logger;
        public CronJobController(ISettingsRepository settingsRepository, ILogger<BinanceSellController> logger)
        {
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        [HttpGet("{cron}")]
        public IActionResult CronCheck(string cronExpression)
        {
            if (string.IsNullOrEmpty(cronExpression))
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            try
            {
                CronExpression expression = CronExpression.Parse(cronExpression);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Неверный формат Cron: {ex}");
                return BadRequest($"Неверный формат Cron: {ex}");
            }
        }

        [HttpGet("{next}")]
        public IEnumerable<string> GetOccurrences(string cronExpression)
        {
            if (string.IsNullOrEmpty(cronExpression))
            {
                return null;
            }

            try
            {
                CronExpression expression = CronExpression.Parse(cronExpression);

                return expression.GetOccurrences(DateTime.UtcNow, DateTime.UtcNow.AddMonths(2)).Take(20).Select(x=>x.ToString()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения следующих дат.");
                return null;
            }
        }
    }
}