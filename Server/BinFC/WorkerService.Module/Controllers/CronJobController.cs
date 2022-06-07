using Cronos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkerService.Module.Cronos;
using WorkerService.Module.Localization;

namespace WorkerService.Module
{
    [Authorize]
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

        [HttpGet("check")]
        public IActionResult CronCheck(string cron)
        {
            if (string.IsNullOrEmpty(cron))
            {
                return BadRequest(WorkerServiceLoc.Empty);
            }

            try
            {
                CronExpression expression = CronParseHelper.GetCronExpression(cron);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(WorkerServiceLoc.IncorrectCronFormat, ex));
                return BadRequest(string.Format(WorkerServiceLoc.IncorrectCronFormat, ex));
            }
        }

        [HttpGet("next")]
        public ActionResult<IEnumerable<string>> GetOccurrences(string cron)
        {
            if (string.IsNullOrEmpty(cron))
            {
                return BadRequest(WorkerServiceLoc.Empty);
            }

            try
            {
                CronExpression expression = CronParseHelper.GetCronExpression(cron);

                if (expression == null)
                {
                    return BadRequest(string.Format(WorkerServiceLoc.IncorrectCronFormat, cron));
                }

                return expression.GetOccurrences(DateTime.UtcNow, DateTime.UtcNow.AddMonths(2)).Take(20).Select(x => x.ToString()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, WorkerServiceLoc.ErrorGetOccurrences);
                return BadRequest(WorkerServiceLoc.ErrorGetOccurrences);
            }
        }
    }
}