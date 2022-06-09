using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Localization;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Base
{
    public class BaseRepository : IBaseRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<BaseRepository> _logger;
        public BaseRepository(DataContext dataContext, ILogger<BaseRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, string Message)> SaveChangesAsync()
        {
            try
            {
                await _dataContext.SaveChangesAsync();
                return (true, StorageLoc.SaveSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Join("; ", _dataContext.ChangeTracker.Entries().Select(x => x.Entity.GetType().Name)));
                _dataContext.ChangeTracker.Clear();
                return (false, StorageLoc.SaveUnsuccess);
            }
        }

        public async Task<UserInfo> GetOrCreateUserInfoAsync(long userId)
        {
            var foundedUserInfo = await GetUserInfoAsync(userId);
            if (foundedUserInfo == null)
            {
                Unique defaultUnique = await GetDefaultUniqueAsync();
                UserInfo userInfo = new()
                {
                    UserId = userId,
                    Unique = defaultUnique
                };

                return userInfo;
            }

            return foundedUserInfo;
        }

        private Task<UserInfo> GetUserInfoAsync(long userId)
        {
            return _dataContext
                    .UsersInfo
                    .Include(i => i.Unique)
                    .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public Task<Unique> GetDefaultUniqueAsync()
        {
            return _dataContext
                    .Unique
                    .FirstOrDefaultAsync(x => x.IsDefault);
        }
    }
}