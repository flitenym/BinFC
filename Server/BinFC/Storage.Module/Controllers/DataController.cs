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
    public class DataController : BaseController
    {
        private readonly ISpotDataRepository _spotDataRepository;
        private readonly IFuturesDataRepository _futuresDataRepository;
        private readonly ILogger<DataController> _logger;
        public DataController(
            ISpotDataRepository spotDataRepository, 
            IFuturesDataRepository futuresDataRepository, 
            ILogger<DataController> logger)
        {
            _spotDataRepository = spotDataRepository;
            _futuresDataRepository = futuresDataRepository;
            _logger = logger;
        }

        [HttpGet("spot")]
        public IEnumerable<SpotData> GetSpot()
        {
            return _spotDataRepository.Get();
        }

        [HttpGet("futures")]
        public IEnumerable<FuturesData> GetFutures()
        {
            return _futuresDataRepository.Get();
        }

        [HttpPost("deletespot")]
        public async Task<IActionResult> DeleteSpot([FromBody] IEnumerable<long> Ids)
        {
            if (Ids == null || !Ids.Any())
            {
                return BadRequest("Не указаны значения.");
            }

            return StringToResult(await _spotDataRepository.DeleteAsync(Ids));
        }

        [HttpPost("deletefutures")]
        public async Task<IActionResult> DeleteFutures([FromBody] IEnumerable<long> Ids)
        {
            if (Ids == null || !Ids.Any())
            {
                return BadRequest("Не указаны значения.");
            }

            return StringToResult(await _futuresDataRepository.DeleteAsync(Ids));
        }
    }
}