using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
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

        public async Task<string> SaveChangesAsync()
        {
            try
            {
                await _dataContext.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Join("; ", _dataContext.ChangeTracker.Entries().Select(x => x.Entity.GetType().Name)));
                _dataContext.ChangeTracker.Clear();
                return ex.Message;
            }
        }

        public async Task<UserInfo> GetOrCreateUserInfoAsync(long userId)
        {
            var foundedUserInfo = GetUserInfo(userId);
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

        private UserInfo GetUserInfo(long userId)
        {
            return _dataContext
                .UsersInfo
                .Include(i => i.Unique)
                .FirstOrDefault(x => x.UserId == userId);
        }

        public async Task<Unique> GetDefaultUniqueAsync()
        {
            return
                await _dataContext
                    .Unique
                    .FirstOrDefaultAsync(x => x.IsDefault);
        }
    }
}