using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Controllers.DTO;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : BaseController
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger<SettingsController> _logger;
        public SettingsController(ISettingsRepository settingsRepository, ILogger<SettingsController> logger)
        {
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Settings> Get()
        {
            return _settingsRepository.Get();
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] IEnumerable<SettingsDTO> settings)
        {
            foreach(var obj in settings)
            {
                string error = await _settingsRepository.SetSettingsByKeyAsync(obj.Key, obj.Value);

                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest(error);
                }
            }

            return StringToResult(await _settingsRepository.SaveChangesAsync());
        }
    }
}