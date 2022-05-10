using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using System.Threading.Tasks;
using WorkerService.Module.Services.Base;
using WorkerService.Module.Services.Intrefaces;

namespace WorkerService.Module
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinanceSellController : BaseController
    {
        private readonly CronJobBaseService<IBinanceSell> _binanceSell;
        private readonly ILogger<BinanceSellController> _logger;
        public BinanceSellController(CronJobBaseService<IBinanceSell> binanceSell, ILogger<BinanceSellController> logger)
        {
            _binanceSell = binanceSell;
            _logger = logger;
        }

        [HttpPost("restart")]
        public async Task<IActionResult> Restart()
        {
            string restartError = await _binanceSell.RestartAsync();

            if (!string.IsNullOrEmpty(restartError))
            {
                return BadRequest(restartError);
            }

            return Ok();
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            string startError = await _binanceSell.StartAsync();

            if (!string.IsNullOrEmpty(startError))
            {
                return BadRequest(startError);
            }

            return Ok();
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop()
        {
            string stopError = await _binanceSell.StopAsync();

            if (!string.IsNullOrEmpty(stopError))
            {
                return BadRequest(stopError);
            }

            return Ok();
        }
    }
}