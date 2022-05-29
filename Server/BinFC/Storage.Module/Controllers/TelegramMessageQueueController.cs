using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramMessageQueueController : BaseController
    {
        private readonly ITelegramMessageQueueRepository _telegramMessageQueueRepository;
        private readonly ILogger<TelegramMessageQueueController> _logger;
        public TelegramMessageQueueController(
            ITelegramMessageQueueRepository telegramMessageQueueRepository, 
            ILogger<TelegramMessageQueueController> logger)
        {
            _telegramMessageQueueRepository = telegramMessageQueueRepository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<TelegramMessageQueue> Get()
        {
            return _telegramMessageQueueRepository.Get();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TelegramMessageQueue obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            return StringToResult(await _telegramMessageQueueRepository.CreateAsync(obj));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var obj = await _telegramMessageQueueRepository.GetByIdAsync(id);

            if (obj == null)
            {
                return NotFound("Не найдена запись для удаления.");
            }

            return StringToResult(await _telegramMessageQueueRepository.DeleteAsync(obj));
        }
    }
}