using Storage.Module.Entities;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ITelegramUserInfoRepository
    {
        public Task<TelegramUserInfo> GetByChatIdAsync(long? chatId);
        public Task<string> CreateAsync(TelegramUserInfo obj);
        public Task<string> UpdateAsync(long? chatId, string lastCommand);
        public Task<string> SaveChangesAsync();
    }
}
