using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using System.Threading.Tasks;

namespace WorkerService.Module
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinanceSellController : BaseController
    {
        private readonly ILogger<BinanceSellController> _logger;
        public BinanceSellController(ILogger<BinanceSellController> logger)
        {
            _logger = logger;
        }

        [HttpPost("restart")]
        public async Task<IActionResult> Restart()
        {
            return Ok();
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            return Ok();
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop()
        {
            return Ok();
        }
    }
}