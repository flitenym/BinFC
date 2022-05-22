using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using Storage.Module.Repositories.Base;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class SpotDataRepository : ISpotDataRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<SpotDataRepository> _logger;
        public SpotDataRepository(DataContext dataContext, IBaseRepository baseRepository, ILogger<SpotDataRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _logger = logger;
        }

        #region Controller methods

        public IEnumerable<SpotData> Get()
        {
            return _dataContext.SpotData;
        }

        public void Create(Data obj)
        {
            SpotData newObj = new SpotData()
            {
                AgentEarnUsdt = obj.AgentEarnUsdt,
                UserId = obj.UserId,
            };

            _dataContext.SpotData.Add(newObj);
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