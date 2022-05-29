using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UniqueController : BaseController
    {
        private readonly IUniqueRepository _uniqueRepository;
        private readonly ILogger<UniqueController> _logger;
        public UniqueController(IUniqueRepository uniqueRepository, ILogger<UniqueController> logger)
        {
            _uniqueRepository = uniqueRepository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Unique> Get()
        {
            return _uniqueRepository.Get();
        }

        [HttpGet("{id}")]
        public async Task<Unique> Get(long id)
        {
            return await _uniqueRepository.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Unique obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            return StringToResult(await _uniqueRepository.CreateAsync(obj));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Unique newObj)
        {
            if (newObj == null || newObj.Id != id)
            {
                return BadRequest($"Отправлены разные значения у сущности и у переданного Id.");
            }

            var obj = await _uniqueRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound($"Не найден {nameof(Unique)} с Id = {newObj.Id}.");
            }

            return StringToResult(await _uniqueRepository.UpdateAsync(obj, newObj));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var obj = await _uniqueRepository.GetByIdAsync(id);

            if (obj == null)
            {
                return NotFound("Не найдена запись для удаления.");
            }

            return StringToResult(await _uniqueRepository.DeleteAsync(obj));
        }
    }
}