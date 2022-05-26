﻿using Microsoft.AspNetCore.Authorization;
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
        public async Task<SpotScale> GetSpotById(long Id)
        {
            return await _spotScaleRepository.GetByIdAsync(Id);
        }

        [HttpGet("futures/{id}")]
        public async Task<FuturesScale> GetFuturesById(long Id)
        {
            return await _futuresScaleRepository.GetByIdAsync(Id);
        }

        [HttpPost("createspot")]
        public async Task<IActionResult> CreateSpot([FromBody] SpotScale obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            return StringToResult(await _spotScaleRepository.CreateAsync(obj));
        }

        [HttpPost("createfutures")]
        public async Task<IActionResult> CreateFutures([FromBody] FuturesScale obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            return StringToResult(await _futuresScaleRepository.CreateAsync(obj));
        }

        [HttpPost("deletespot")]
        public async Task<IActionResult> DeleteSpot([FromBody] IEnumerable<long> Ids)
        {
            if (Ids == null || !Ids.Any())
            {
                return BadRequest("Не указаны значения.");
            }

            return StringToResult(await _spotScaleRepository.DeleteAsync(Ids));
        }

        [HttpPost("deletefutures")]
        public async Task<IActionResult> DeleteFutures([FromBody] IEnumerable<long> Ids)
        {
            if (Ids == null || !Ids.Any())
            {
                return BadRequest("Не указаны значения.");
            }

            return StringToResult(await _futuresScaleRepository.DeleteAsync(Ids));
        }

        [HttpPut("updatespot/{id}")]
        public async Task<IActionResult> UpdateSpot(long Id, [FromBody] SpotScale newObj)
        {
            if (newObj == null || newObj.Id != Id)
            {
                return BadRequest($"Отправлены разные значения у сущности и у переданного Id.");
            }

            var obj = await _spotScaleRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound($"Не найден {nameof(SpotScale)} с Id = {newObj.Id}.");
            }

            return StringToResult(await _spotScaleRepository.UpdateAsync(obj, newObj));
        }

        [HttpPut("updatefutures/{id}")]
        public async Task<IActionResult> UpdateFutures(long Id, [FromBody] FuturesScale newObj)
        {
            if (newObj == null || newObj.Id != Id)
            {
                return BadRequest($"Отправлены разные значения у сущности и у переданного Id.");
            }

            var obj = await _futuresScaleRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound($"Не найден {nameof(FuturesScale)} с Id = {newObj.Id}.");
            }

            return StringToResult(await _futuresScaleRepository.UpdateAsync(obj, newObj));
        }
    }
}