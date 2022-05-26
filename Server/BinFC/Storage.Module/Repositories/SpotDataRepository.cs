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
            UserInfo userInfo = _baseRepository.GetOrCreateUserInfo(obj.UserId);

            SpotData newObj = new()
            {
                AgentEarnUsdt = obj.AgentEarnUsdt,
                User = userInfo,
                LoadingDate = obj.LoadingDate
            };

            _dataContext.SpotData.Add(newObj);
        }

        public async Task<string> DeleteAsync(IEnumerable<long> Ids)
        {
            _dataContext
                .SpotData
                .RemoveRange(
                    _dataContext
                    .SpotData
                    .Where(x => Ids.Contains(x.Id))
                );

            return await SaveChangesAsync();
        }

        public async Task<string> DeleteAllAsync()
        {
            _dataContext
                .SpotData
                .RemoveRange(
                    _dataContext
                    .SpotData.ToArray()
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