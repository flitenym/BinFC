using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System;
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
        private readonly ITelegramMessageQueueRepository _telegramMessageQueueRepository;
        private readonly ILogger<UserInfoRepository> _logger;
        public UserInfoRepository(
            DataContext dataContext,
            IBaseRepository baseRepository,
            IUniqueRepository uniqueRepository,
            ITelegramMessageQueueRepository telegramMessageQueueRepository,
            ILogger<UserInfoRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _uniqueRepository = uniqueRepository;
            _telegramMessageQueueRepository = telegramMessageQueueRepository;
            _logger = logger;
        }

        #region Controller methods

        public IEnumerable<UserInfo> Get()
        {
            return _dataContext
                .UsersInfo
                .Include(i => i.Unique)
                .OrderBy(x => x.IsApproved)
                .ThenBy(x => x.Id);
        }

        public IEnumerable<UserInfo> Get(DateTime beforeDate)
        {
            return _dataContext
                .UsersInfo
                .Include(i => i.Unique)
                .Where(x => x.Created < beforeDate)
                .OrderBy(x => x.Id);
        }

        public async Task<UserInfo> GetByIdAsync(long id)
        {
            return await _dataContext
                .UsersInfo
                .Include(i => i.Unique)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public List<long> GetChatIdByUserNickName(List<string> userNickNames)
        {
            return _dataContext
                .UsersInfo
                .Where(x => userNickNames.Contains(x.UserNickName) && x.ChatId.HasValue)
                .Select(x => x.ChatId.Value)
                .ToList();
        }

        public async Task<string> CreateAsync(UserInfo obj)
        {
            if (!obj.UniqueId.HasValue)
            {
                Unique defaultUnique = await _baseRepository.GetDefaultUniqueAsync();

                obj.UniqueId = defaultUnique.Id;
            }

            _dataContext.UsersInfo.Add(obj);

            return await SaveChangesAsync();
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

            if (newObj.UserNickName != null)
            {
                obj.UserNickName = newObj.UserNickName;
            }

            if (newObj.UniqueId.HasValue)
            {
                var unique = await _uniqueRepository.GetByIdAsync(newObj.UniqueId.Value);

                if (unique != null)
                {
                    obj.UniqueId = newObj.UniqueId;
                }
            }

            _dataContext.UsersInfo.Update(obj);

            return await SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(UserInfo obj)
        {
            _dataContext.UsersInfo.Remove(obj);

            return await SaveChangesAsync();
        }

        public async Task<string> ApproveAsync(IEnumerable<long> ids)
        {
            foreach (var id in ids)
            {
                var userInfoById = await GetByIdAsync(id);

                userInfoById.IsApproved = true;

                if (userInfoById.ChatId != null)
                {
                    _telegramMessageQueueRepository.Create(new TelegramMessageQueue()
                    {
                        ChatId = userInfoById.ChatId,
                        Message = $"Ваш id ({userInfoById.UserId}) подтвержден Администратором."
                    });
                }
            }

            return await SaveChangesAsync();
        }

        public async Task<string> NotApproveAsync(IEnumerable<long> ids)
        {
            foreach (var id in ids)
            {
                var userInfoById = await GetByIdAsync(id);

                userInfoById.IsApproved = false;

                if (userInfoById.ChatId != null)
                {
                    _telegramMessageQueueRepository.Create(new TelegramMessageQueue()
                    {
                        ChatId = userInfoById.ChatId,
                        Message = $"Ваш id ({userInfoById.UserId}) не подтвержден Администратором."
                    });
                }

                _dataContext.Remove(userInfoById);
            }

            return await SaveChangesAsync();
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