using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminController> _logger;
        public AdminController(IAdminRepository adminRepository, ILogger<AdminController> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Admin> Get()
        {
            return _adminRepository.Get();
        }

        [HttpGet("{id}")]
        public async Task<Admin> Get(long Id)
        {
            return await _adminRepository.GetByIdAsync(Id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Admin obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            return StringToResult(await _adminRepository.CreateAsync(obj));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Admin obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            if (await _adminRepository.LoginAsync(obj))
            {
                return Ok();
            }
            else
            {
                return NotFound("Неверно указаны логин или пароль");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long Id, [FromBody] Admin newObj)
        {
            if (newObj == null || newObj.Id != Id)
            {
                return BadRequest($"Отправлены разные значения у сущности и у переданного Id.");
            }

            var obj = await _adminRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound($"Не найден {nameof(UserInfo)} с Id = {newObj.Id}.");
            }

            return StringToResult(await _adminRepository.UpdateAsync(obj, newObj));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long Id)
        {
            var obj = await _adminRepository.GetByIdAsync(Id);

            if (obj == null)
            {
                return NotFound("Не найдена запись для удаления.");
            }

            return StringToResult(await _adminRepository.DeleteAsync(obj));
        }
    }
}