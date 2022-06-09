using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class TelegramMessageQueueRepository : ITelegramMessageQueueRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<TelegramMessageQueueRepository> _logger;
        public TelegramMessageQueueRepository(
            DataContext dataContext,
            IBaseRepository baseRepository,
            ILogger<TelegramMessageQueueRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _logger = logger;
        }

        public IEnumerable<TelegramMessageQueue> Get()
        {
            return _dataContext
                .TelegramMessagesQueue
                .OrderBy(x => x.Id);
        }

        public Task<TelegramMessageQueue> GetByIdAsync(long id)
        {
            return _dataContext
                .TelegramMessagesQueue
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<(bool IsSuccess, string Message)> CreateAsync(TelegramMessageQueue obj)
        {
            _dataContext.TelegramMessagesQueue.Add(obj);

            return SaveChangesAsync();
        }

        public void Create(TelegramMessageQueue obj)
        {
            _dataContext.TelegramMessagesQueue.Add(obj);
        }

        public Task<(bool IsSuccess, string Message)> DeleteAsync(TelegramMessageQueue obj)
        {
            _dataContext.TelegramMessagesQueue.Remove(obj);

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> DeleteAsync(long id)
        {
            _dataContext
                .TelegramMessagesQueue
                .RemoveRange(
                    _dataContext
                    .TelegramMessagesQueue
                    .Where(x => x.Id == id)
                );

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }
    }
}