using FatCamel.Host.Core;
using Microsoft.EntityFrameworkCore;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly DataContext _dataContext;
        public UserInfoRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task DataContextSaveChanges()
        {
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                StartupLogger.LogError(ex,
                    nameof(UserInfoRepository) + string.Join("; ", _dataContext.ChangeTracker.Entries().Select(x => x.Entity.GetType().Name)));
                _dataContext.ChangeTracker.Clear();
            }
        }

        public async Task<UserInfo> GetUserInfoByChatId(long chatId, bool isNeedTracking = true)
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

        public async Task<UserInfo> GetUserInfoByUserId(long userId, bool isNeedTracking = true)
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
    }
}