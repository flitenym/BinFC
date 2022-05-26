using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using Storage.Module.Repositories.Base;
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
                .OrderBy(x => x.Id);
        }

        public void Create(Data obj)
        {
            UserInfo userInfo = _baseRepository.GetOrCreateUserInfo(obj.UserId);

            FuturesData newObj = new()
            {
                AgentEarnUsdt = obj.AgentEarnUsdt,
                User = userInfo,
                LoadingDate = obj.LoadingDate
            };

            _dataContext.FuturesData.Add(newObj);
        }

        public async Task<string> DeleteAsync(IEnumerable<long> Ids)
        {
            _dataContext
                .FuturesData
                .RemoveRange(
                    _dataContext
                    .FuturesData
                    .Where(x => Ids.Contains(x.Id))
                );

            return await SaveChangesAsync();
        }

        public async Task<string> DeleteAllAsync()
        {
            _dataContext
                .FuturesData
                .RemoveRange(
                    _dataContext
                    .FuturesData.ToArray()
                );

            return await SaveChangesAsync();
        }

        public Task<string> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }

        #endregion
    }
}