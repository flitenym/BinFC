using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ITelegramMessageQueueRepository
    {
        public IEnumerable<TelegramMessageQueue> Get();
        public Task<TelegramMessageQueue> GetByIdAsync(long id);
        public Task<(bool IsSuccess, string Message)> CreateAsync(TelegramMessageQueue obj);
        public void Create(TelegramMessageQueue obj);
        public Task<(bool IsSuccess, string Message)> DeleteAsync(TelegramMessageQueue obj);
        public Task<(bool IsSuccess, string Message)> DeleteAsync(long id);
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
    }
}