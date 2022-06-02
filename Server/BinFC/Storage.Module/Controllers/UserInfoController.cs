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
    public class UserInfoController : BaseController
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly ILogger<UserInfoController> _logger;
        public UserInfoController(IUserInfoRepository userInfoRepository, ILogger<UserInfoController> logger)
        {
            _userInfoRepository = userInfoRepository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<UserInfo> Get()
        {
            return _userInfoRepository.Get();
        }

        [HttpGet("{id}")]
        public async Task<UserInfo> Get(long id)
        {
            return await _userInfoRepository.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserInfo obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            return StringToResult(await _userInfoRepository.CreateAsync(obj));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UserInfo newObj)
        {
            if (newObj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            if (newObj.Id != id)
            {
                return BadRequest($"Отправлены разные значения у сущности и у переданного Id.");
            }

            var obj = await _userInfoRepository.GetByIdAsync(newObj.Id);

            if (obj == null)
            {
                return NotFound($"Не найден {nameof(UserInfo)} с Id = {newObj.Id}.");
            }

            return StringToResult(await _userInfoRepository.UpdateAsync(obj, newObj));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var obj = await _userInfoRepository.GetByIdAsync(id);

            if (obj == null)
            {
                return NotFound("Не найдена запись для удаления.");
            }

            return StringToResult(await _userInfoRepository.DeleteAsync(obj));
        }

        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromBody] IEnumerable<long> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Не указаны значения.");
            }

            return StringToResult(await _userInfoRepository.ApproveAsync(ids));
        }

        [HttpPost("notapprove")]
        public async Task<IActionResult> NotApprove([FromBody] IEnumerable<long> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Не указаны значения.");
            }

            return StringToResult(await _userInfoRepository.NotApproveAsync(ids));
        }
    }
}