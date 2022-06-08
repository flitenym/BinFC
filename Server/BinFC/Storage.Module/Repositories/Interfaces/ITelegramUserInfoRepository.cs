using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ITelegramUserInfoRepository
    {
        public IEnumerable<TelegramUserInfo> Get();
        public Task<TelegramUserInfo> GetByChatIdAsync(long? chatId, bool isNeedTracking = true);
        public Task<(bool IsSuccess, string Message)> CreateAsync(TelegramUserInfo obj);
        public Task<(bool IsSuccess, string Message)> UpdateAsync(TelegramUserInfo obj);
        public Task<(bool IsSuccess, string Message)> UpdateAsync(long? chatId, string lastCommand);
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
    }
}