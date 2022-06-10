using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class FuturesDataRepository : IFuturesDataRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<FuturesDataRepository> _logger;
        public FuturesDataRepository(DataContext dataContext, IBaseRepository baseRepository, ILogger<FuturesDataRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _logger = logger;
        }

        #region Controller methods

        public IEnumerable<FuturesData> Get()
        {
            return _dataContext
                .FuturesData
                .Include(i => i.User)
                .OrderBy(x => x.Id);
        }

        public IEnumerable<FuturesData> GetLastData()
        {
            return _dataContext.FuturesData.AsNoTracking()
                .Select(t => t.LoadingDate)
                .Distinct()
                .OrderByDescending(t => t)
                .Take(2)
                .SelectMany(key =>
                    _dataContext
                    .FuturesData
                    .Include(i => i.User)
                    .AsNoTracking()
                    .Where(t => t.LoadingDate == key)
            );
        }

        public async Task CreateAsync(Data obj)
        {
            UserInfo userInfo = await _baseRepository.GetOrCreateUserInfoAsync(obj.UserId);

            FuturesData newObj = new()
            {
                AgentEarnUsdt = obj.AgentEarnUsdt,
                User = userInfo,
                LoadingDate = obj.LoadingDate
            };

            _dataContext.FuturesData.Add(newObj);
        }

        public Task<(bool IsSuccess, string Message)> DeleteAsync(IEnumerable<long> ids)
        {
            _dataContext
                .FuturesData
                .RemoveRange(
                    _dataContext
                    .FuturesData
                    .Where(x => ids.Contains(x.Id))
                );

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> DeleteAllAsync()
        {
            _dataContext
                .FuturesData
                .RemoveRange(
                    _dataContext
                    .FuturesData.ToArray()
                );

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }

        #endregion

        public void UpdateIsPaidByUserId(long userId)
        {
            var records = GetByUserId(userId);

            if (!records.Any())
            {
                return;
            }

            foreach (var record in records)
            {
                if (!record.IsPaid)
                {
                    record.IsPaid = true;
                    _dataContext.FuturesData.Update(record);
                }
            }
        }

        private IEnumerable<FuturesData> GetByUserId(long userId)
        {
            return _dataContext
                .FuturesData
                .Include(i => i.User)
                .Where(x => x.User.UserId == userId);
        }
    }
}