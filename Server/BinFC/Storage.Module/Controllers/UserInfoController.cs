using FatCamel.Host.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<UserInfoController> _logger;
        public UserInfoController(DataContext dataContext, ILogger<UserInfoController> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<UserInfo> Get()
        {
            return _dataContext.UsersInfo;
        }

        [HttpGet("{id}")]
        public async Task<UserInfo> Get(long Id)
        {
            return await GetObj(Id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserInfo obj)
        {
            if (obj == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            _dataContext.UsersInfo.Add(obj);

            return await SaveChangesAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long Id, [FromBody] UserInfo obj)
        {
            if (obj == null || obj.Id != Id)
            {
                return BadRequest($"Отправлены разные значения у сущности и у переданного Id.");
            }

            var findedObj = await GetObj(obj.Id);

            if (findedObj == null)
            {
                return NotFound($"Не найден {nameof(UserInfo)} с Id = {obj.Id}.");
            }

            findedObj.ChatId = obj.ChatId;
            findedObj.UserId = obj.UserId;
            findedObj.UserName = obj.UserName;
            findedObj.UserEmail = obj.UserEmail;
            findedObj.TrcAddress = obj.TrcAddress;
            findedObj.BepAddress = obj.BepAddress;
            findedObj.UniqueString = obj.UniqueString;

            _dataContext.UsersInfo.Update(findedObj);

            return await SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long Id)
        {
            UserInfo obj = await GetObj(Id);

            if (obj == null)
            {
                return NotFound("Не найдена запись для удаления.");
            }

            _dataContext.UsersInfo.Remove(obj);

            return await SaveChangesAsync();
        }

        private async Task<UserInfo> GetObj(long Id)
        {
            return await _dataContext.UsersInfo.FindAsync(Id);
        }

        private async Task<IActionResult> SaveChangesAsync()
        {
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Join("; ", _dataContext.ChangeTracker.Entries().Select(x => x.Entity.GetType().Name)));
                _dataContext.ChangeTracker.Clear();
                return BadRequest($"Не удалось выполнить сохранение: {ex.Message}");
            }
        }
    }
}