using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ITelegramUserInfoRepository
    {
        public IEnumerable<TelegramUserInfo> Get();
        public Task<TelegramUserInfo> GetByChatIdAsync(long? chatId, bool isNeedTracking = true);
        public Task<string> CreateAsync(TelegramUserInfo obj);
        public Task<string> UpdateAsync(TelegramUserInfo obj);
        public Task<string> UpdateAsync(long? chatId, string lastCommand);
        public Task<string> SaveChangesAsync();
    }
}