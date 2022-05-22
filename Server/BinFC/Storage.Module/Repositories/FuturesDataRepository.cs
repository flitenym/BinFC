using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using Storage.Module.Repositories.Base;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
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
            return _dataContext.FuturesData;
        }

        public void Create(Data obj)
        {
            _baseRepository.CreateUserInfo(obj.UserId);

            FuturesData newObj = new FuturesData()
            {
                AgentEarnUsdt = obj.AgentEarnUsdt,
                UserId = obj.UserId,
            };

            _dataContext.FuturesData.Add(newObj);
        }

        public async Task<string> DeleteAsync()
        {

            return await SaveChangesAsync();
        }

        public Task<string> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }

        #endregion
    }
}