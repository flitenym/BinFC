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

        public UserInfo GetOrCreateUserInfo(long userId)
        {
            var foundedUserInfo = GetUserInfo(userId);
            if (foundedUserInfo == null)
            {

                UserInfo userInfo = new()
                {
                    UserId = userId,
                    Unique = GetDefaultUnique()
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

        private Unique GetDefaultUnique()
        {
            return
                _dataContext
                    .Unique
                    .FirstOrDefault(x => x.Id == DefaultValues.UniqueId) ??
                _dataContext
                    .Unique
                    .FirstOrDefault(x => x.Name == DefaultValues.UniqueName) ??
                _dataContext
                    .Unique
                    .FirstOrDefault();
        }
    }
}