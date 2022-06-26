using HostLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Controllers.DTO;
using Storage.Module.Entities;
using Storage.Module.Localization;
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
        private readonly IJwtAuthService _jwtAuthService;
        private readonly ILogger<AdminController> _logger;
        public AdminController(IAdminRepository adminRepository, IJwtAuthService jwtAuthService, ILogger<AdminController> logger)
        {
            _adminRepository = adminRepository;
            _jwtAuthService = jwtAuthService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public IEnumerable<Admin> Get()
        {
            return _adminRepository.Get();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<Admin> GetAsync(long id)
        {
            return await _adminRepository.GetByIdAsync(id);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Admin obj)
        {
            if (obj == null)
            {
                return BadRequest(StorageLoc.Empty);
            }

            return StringToResult(await _adminRepository.CreateAsync(obj));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] Admin obj)
        {
            if (obj == null)
            {
                return BadRequest(StorageLoc.Empty);
            }

            if (await _adminRepository.LoginAsync(obj))
            {
                string encodedJwt = _jwtAuthService.GetToken(obj.UserName);

                var response = new
                {
                    username = obj.UserName,
                    token = encodedJwt
                };

                return Ok(response);
            }
            else
            {
                return BadRequest(StorageLoc.IncorrectLoginOrPassword);
            }
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] AdminDTO obj)
        {
            if (obj == null)
            {
                return BadRequest(StorageLoc.Empty);
            }

            return StringToResult(await _adminRepository.ChangePasswordAsync(obj.UserName, obj.OldPassword, obj.NewPassword));
        }

        [Authorize]
        [HttpPut("updatelanguage")]
        public async Task<IActionResult> UpdateLanguageAsync(string userName, string language)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(StorageLoc.EmptyLogin);
            }

            return StringToResult(await _adminRepository.UpdateLanguageAsync(userName, language));
        }

        [Authorize]
        [HttpGet("getlanguage")]
        public async Task<IActionResult> GetLanguageAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(StorageLoc.EmptyLogin);
            }

            (bool isSuccess, string message) = await _adminRepository.GetLanguageAsync(userName);

            if (isSuccess)
            {
                return Ok(message);
            }

            return BadRequest(message);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] Admin newObj)
        {
            if (newObj == null || newObj.Id != id)
            {
                return BadRequest(StorageLoc.NotEqualIds);
            }

            var obj = await _adminRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound(string.Format(StorageLoc.NotFoundWithId, nameof(UserInfo), newObj.Id));
            }

            return StringToResult(await _adminRepository.UpdateAsync(obj, newObj));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var obj = await _adminRepository.GetByIdAsync(id);

            if (obj == null)
            {
                return NotFound(StorageLoc.NotFoundForRemove);
            }

            return StringToResult(await _adminRepository.DeleteAsync(obj));
        }
    }
}