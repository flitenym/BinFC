using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Entities;
using Storage.Module.Localization;
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
                return BadRequest(StorageLoc.Empty);
            }

            return StringToResult(await _uniqueRepository.CreateAsync(obj));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Unique newObj)
        {
            if (newObj == null || newObj.Id != id)
            {
                return BadRequest(StorageLoc.NotEqualIds);
            }

            var obj = await _uniqueRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound(string.Format(StorageLoc.NotFoundWithId, nameof(Unique), newObj.Id));
            }

            return StringToResult(await _uniqueRepository.UpdateAsync(obj, newObj));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var obj = await _uniqueRepository.GetByIdAsync(id);

            if (obj.IsDefault)
            {
                return BadRequest(StorageLoc.CanNotRemoveDefault);
            }

            if (obj == null)
            {
                return NotFound(StorageLoc.NotFoundForRemove);
            }

            return StringToResult(await _uniqueRepository.DeleteAsync(obj));
        }
    }
}