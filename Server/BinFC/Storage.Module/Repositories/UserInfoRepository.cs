using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly IUniqueRepository _uniqueRepository;
        private readonly ILogger<UserInfoRepository> _logger;
        public UserInfoRepository(
            DataContext dataContext, 
            IBaseRepository baseRepository, 
            IUniqueRepository uniqueRepository, 
            ILogger<UserInfoRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _uniqueRepository = uniqueRepository;
            _logger = logger;
        }

        #region Controller methods

        public IEnumerable<UserInfo> Get()
        {
            return _dataContext
                .UsersInfo
                .Include(i => i.Unique);
        }

        public async Task<List<UserInfo>> GetAdminsAsync()
        {
            return await _dataContext
                .UsersInfo
                .Include(i => i.Unique)
                .AsNoTracking()
                .Where(x => x.IsAdmin)
                .Where(x => x.ChatId.HasValue)
                .ToListAsync();
        }

        public async Task<UserInfo> GetByIdAsync(long Id)
        {
            return await _dataContext
                .UsersInfo
                .Include(i => i.Unique)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<string> CreateAsync(UserInfo obj)
        {
            _dataContext.UsersInfo.Add(obj);

            return await _baseRepository.SaveChangesAsync();
        }

        public async Task<string> UpdateAsync(UserInfo obj, UserInfo newObj)
        {
            obj.ChatId = newObj.ChatId;
            obj.UserId = newObj.UserId;
            obj.UserName = newObj.UserName;
            obj.UserEmail = newObj.UserEmail;
            obj.TrcAddress = newObj.TrcAddress;
            obj.BepAddress = newObj.BepAddress;
             
            var unique = await _uniqueRepository.GetByIdAsync(obj.UniqueId);

            if (unique == null)
            {
                return "Не найден уникальный ключ у пользователя Unique.";
            }

            obj.UniqueId = newObj.UniqueId; 
            
            _dataContext.UsersInfo.Update(obj);

            return await _baseRepository.SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(UserInfo obj)
        {
            _dataContext.UsersInfo.Remove(obj);

            return await _baseRepository.SaveChangesAsync();
        }

        #endregion

        public async Task<UserInfo> GetUserInfoByChatIdAsync(long chatId, bool isNeedTracking = true)
        {
            if (isNeedTracking)
            {
                return await _dataContext.UsersInfo
                    .Where(x => x.ChatId == chatId)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return await _dataContext.UsersInfo
                    .AsNoTracking()
                    .Where(x => x.ChatId == chatId)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<UserInfo> GetUserInfoByUserIdAsync(long userId, bool isNeedTracking = true)
        {
            if (isNeedTracking)
            {
                return await _dataContext.UsersInfo
                    .Where(x => x.UserId == userId)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return await _dataContext.UsersInfo
                    .AsNoTracking()
                    .Where(x => x.UserId == userId)
                    .FirstOrDefaultAsync();
            }
        }

        public Task<string> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }
    }
}