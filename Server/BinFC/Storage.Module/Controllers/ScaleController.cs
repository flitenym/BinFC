using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Entities;
using Storage.Module.Localization;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScaleController : BaseController
    {
        private readonly ISpotScaleRepository _spotScaleRepository;
        private readonly IFuturesScaleRepository _futuresScaleRepository;
        private readonly ILogger<ScaleController> _logger;
        public ScaleController(
            ISpotScaleRepository spotScaleRepository, 
            IFuturesScaleRepository futuresScaleRepository, 
            ILogger<ScaleController> logger)
        {
            _spotScaleRepository = spotScaleRepository;
            _futuresScaleRepository = futuresScaleRepository;
            _logger = logger;
        }

        [HttpGet("spot")]
        public IEnumerable<SpotScale> GetSpot()
        {
            return _spotScaleRepository.Get();
        }

        [HttpGet("futures")]
        public IEnumerable<FuturesScale> GetFutures()
        {
            return _futuresScaleRepository.Get();
        }

        [HttpGet("spot/{id}")]
        public async Task<SpotScale> GetSpotById(long id)
        {
            return await _spotScaleRepository.GetByIdAsync(id);
        }

        [HttpGet("futures/{id}")]
        public async Task<FuturesScale> GetFuturesById(long id)
        {
            return await _futuresScaleRepository.GetByIdAsync(id);
        }

        [HttpPost("createspot")]
        public async Task<IActionResult> CreateSpot([FromBody] SpotScale obj)
        {
            if (obj == null)
            {
                return BadRequest(StorageLoc.Empty);
            }

            return StringToResult(await _spotScaleRepository.CreateAsync(obj));
        }

        [HttpPost("createfutures")]
        public async Task<IActionResult> CreateFutures([FromBody] FuturesScale obj)
        {
            if (obj == null)
            {
                return BadRequest(StorageLoc.Empty);
            }

            return StringToResult(await _futuresScaleRepository.CreateAsync(obj));
        }

        [HttpPost("deletespot")]
        public async Task<IActionResult> DeleteSpot([FromBody] IEnumerable<long> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest(StorageLoc.EmptyValues);
            }

            return StringToResult(await _spotScaleRepository.DeleteAsync(ids));
        }

        [HttpPost("deletefutures")]
        public async Task<IActionResult> DeleteFutures([FromBody] IEnumerable<long> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest(StorageLoc.EmptyValues);
            }

            return StringToResult(await _futuresScaleRepository.DeleteAsync(ids));
        }

        [HttpPut("updatespot/{id}")]
        public async Task<IActionResult> UpdateSpot(long id, [FromBody] SpotScale newObj)
        {
            if (newObj == null || newObj.Id != id)
            {
                return BadRequest(StorageLoc.NotEqualIds);
            }

            var obj = await _spotScaleRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound(string.Format(StorageLoc.NotFoundWithId, nameof(SpotScale), newObj.Id));
            }

            return StringToResult(await _spotScaleRepository.UpdateAsync(obj, newObj));
        }

        [HttpPut("updatefutures/{id}")]
        public async Task<IActionResult> UpdateFutures(long id, [FromBody] FuturesScale newObj)
        {
            if (newObj == null || newObj.Id != id)
            {
                return BadRequest(StorageLoc.NotEqualIds);
            }

            var obj = await _futuresScaleRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound(string.Format(StorageLoc.NotFoundWithId, nameof(FuturesScale), newObj.Id));
            }

            return StringToResult(await _futuresScaleRepository.UpdateAsync(obj, newObj));
        }
    }
}