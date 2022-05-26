using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class PayHistoryRepository : IPayHistoryRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<PayHistoryRepository> _logger;
        public PayHistoryRepository(DataContext dataContext, IBaseRepository baseRepository, ILogger<PayHistoryRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _logger = logger;
        }

        public IEnumerable<PayHistory> Get()
        {
            return _dataContext.PayHistory;
        }

        public Task<string> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }
    }
}