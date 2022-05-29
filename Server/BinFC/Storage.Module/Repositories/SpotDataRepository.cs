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
            return _dataContext
                .SpotData
                .OrderBy(x => x.Id);
        }

        public async Task CreateAsync(Data obj)
        {
            UserInfo userInfo = await _baseRepository.GetOrCreateUserInfoAsync(obj.UserId);

            SpotData newObj = new()
            {
                AgentEarnUsdt = obj.AgentEarnUsdt,
                User = userInfo,
                LoadingDate = obj.LoadingDate
            };

            _dataContext.SpotData.Add(newObj);
        }

        public async Task<string> DeleteAsync(IEnumerable<long> ids)
        {
            _dataContext
                .SpotData
                .RemoveRange(
                    _dataContext
                    .SpotData
                    .Where(x => ids.Contains(x.Id))
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