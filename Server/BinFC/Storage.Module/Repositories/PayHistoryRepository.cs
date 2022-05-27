﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
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
            return _dataContext
                .PayHistory
                .Include(i => i.User)
                .OrderBy(x => x.Id);
        }

        public IEnumerable<PayHistory> Get(long[] ids)
        {
            return _dataContext
                .PayHistory
                .Include(i => i.User)
                .Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.Id);
        }

        public async Task<string> DeleteAllAsync()
        {
            _dataContext
                .PayHistory
                .RemoveRange(
                    _dataContext
                    .PayHistory.ToArray()
                );

            return await SaveChangesAsync();
        }

        public async Task<string> CreateAsync(PayHistory obj)
        {
            _dataContext.PayHistory.Add(obj);

            return await SaveChangesAsync();
        }

        public Task<string> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }
    }
}