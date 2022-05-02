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
            await _binanceSell.RestartAsync();
            return Ok();
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            await _binanceSell.StartAsync();
            return Ok();
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop()
        {
            await _binanceSell.StopAsync();
            return Ok();
        }
    }
}