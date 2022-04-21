using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Controllers
{
    [ApiController]
    [Route("fatcamelbot")]
    public class TelegramFatCamelBotController : ControllerBase
    {
        private readonly ITelegramFatCamelBotService _telegramFatCamelBotService;
        private readonly ILogger<TelegramFatCamelBotController> _logger;
        public TelegramFatCamelBotController(ITelegramFatCamelBotService telegramFatCamelBotService, ILogger<TelegramFatCamelBotController> logger)
        {
            _telegramFatCamelBotService = telegramFatCamelBotService;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<IActionResult> FatCamelBotStartAsync()
        {
            await _telegramFatCamelBotService.FatCamelBotStartAsync();
            return Ok();
        }

        [HttpPost("stop")]
        public async Task<IActionResult> FatCamelBotStopAsync()
        {
            await _telegramFatCamelBotService.FatCamelBotStopAsync();
            return Ok();
        }
    }
}
