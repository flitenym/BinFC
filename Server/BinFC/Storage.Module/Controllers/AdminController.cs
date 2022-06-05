using FatCamel.Host.StaticClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Storage.Module.Controllers.Base;
using Storage.Module.Controllers.DTO;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                return BadRequest("Отправлена пустая сущность.");
            }

            return StringToResult(await _adminRepository.CreateAsync(obj));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] Admin obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            if (await _adminRepository.LoginAsync(obj))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, obj.UserName)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: claimsIdentity.Claims,
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    username = obj.UserName,
                    token = encodedJwt
                };

                return Ok(response);
            }
            else
            {
                return BadRequest("Неверно указаны логин или пароль");
            }
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] AdminDTO obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            return StringToResult(await _adminRepository.ChangePasswordAsync(obj.UserName, obj.OldPassword, obj.NewPassword));
        }

        [Authorize]
        [HttpPut("updatelanguage")]
        public async Task<IActionResult> UpdateLanguageAsync(string userName, string language)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest($"Не указан логин");
            }

            return StringToResult(await _adminRepository.UpdateLanguageAsync(userName, language));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] Admin newObj)
        {
            if (newObj == null || newObj.Id != id)
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var obj = await _adminRepository.GetByIdAsync(id);

            if (obj == null)
            {
                return NotFound("Не найдена запись для удаления.");
            }

            return StringToResult(await _adminRepository.DeleteAsync(obj));
        }
    }
}