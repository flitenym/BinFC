using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using System.Threading.Tasks;
using WorkerService.Module.Services;
using WorkerService.Module.Services.Base;
using WorkerService.Module.Services.Intrefaces;

namespace WorkerService.Module
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BinanceSellController : BaseController
    {
        private readonly CronJobBaseService<IBinanceSellService> _binanceSellService;
        private readonly ILogger<BinanceSellController> _logger;
        public BinanceSellController(CronJobBaseService<IBinanceSellService> binanceSellService, ILogger<BinanceSellController> logger)
        {
            _binanceSellService = binanceSellService;
            _logger = logger;
        }

        [HttpGet("restart")]
        public async Task<IActionResult> Restart()
        {
            await _binanceSellService.RestartAsync(default);
            return Ok();
        }

        [HttpGet("start")]
        public async Task<IActionResult> Start()
        {
            await _binanceSellService.StartAsync(default);
            return Ok();
        }

        [HttpGet("stop")]
        public async Task<IActionResult> Stop()
        {
            await _binanceSellService.StopAsync(default);
            return Ok();
        }
    }
}