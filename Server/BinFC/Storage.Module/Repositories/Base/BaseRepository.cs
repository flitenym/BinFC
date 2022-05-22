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

        public void CreateUserInfo(long userId)
        {
            if (IsNeedCreateUser(userId))
            {
                UserInfo userInfo = new UserInfo();
                userInfo.UserId = userId;
                _dataContext.UsersInfo.Add(userInfo);
            }
        }

        private bool IsNeedCreateUser(long userId)
        {
            return _dataContext.UsersInfo.FirstOrDefault(x => x.UserId == userId) == null;
        }
    }
}
