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
                .Include(i => i.Unique)
                .OrderBy(x => x.Id);
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
            if (newObj.ChatId != null)
            {
                obj.ChatId = newObj.ChatId;
            }

            if (newObj.UserId != null)
            {
                obj.UserId = newObj.UserId;
            }

            if (newObj.UserName != null)
            {
                obj.UserName = newObj.UserName;
            }

            if (newObj.UserEmail != null)
            {
                obj.UserEmail = newObj.UserEmail;
            }

            if (newObj.TrcAddress != null)
            {
                obj.TrcAddress = newObj.TrcAddress;
            }

            if (newObj.BepAddress != null)
            {
                obj.BepAddress = newObj.BepAddress;
            }

            obj.IsAdmin = newObj.IsAdmin;

            var unique = await _uniqueRepository.GetByIdAsync(newObj.UniqueId);

            if (unique != null)
            {
                obj.UniqueId = newObj.UniqueId;
            }

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