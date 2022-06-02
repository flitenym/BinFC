using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class SpotScaleRepository : ISpotScaleRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly IUniqueRepository _uniqueRepository;
        private readonly ILogger<SpotScaleRepository> _logger;
        public SpotScaleRepository(
            DataContext dataContext,
            IBaseRepository baseRepository,
            IUniqueRepository uniqueRepository,
            ILogger<SpotScaleRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _uniqueRepository = uniqueRepository;
            _logger = logger;
        }

        #region Controller methods

        public IEnumerable<SpotScale> Get()
        {
            return _dataContext
                .SpotScale
                .Include(i => i.Unique)
                .OrderBy(x => x.Id);
        }

        public async Task<SpotScale> GetByIdAsync(long id)
        {
            return await _dataContext
                .SpotScale
                .Include(i => i.Unique)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<string> CreateAsync(SpotScale obj)
        {
            _dataContext.SpotScale.Add(obj);

            return await SaveChangesAsync();
        }

        public async Task<string> UpdateAsync(SpotScale obj, SpotScale newObj)
        {
            obj.FromValue = newObj.FromValue;
            obj.Percent = newObj.Percent;

            var unique = await _uniqueRepository.GetByIdAsync(newObj.UniqueId);

            if (unique != null)
            {
                obj.UniqueId = newObj.UniqueId;
            }

            _dataContext.SpotScale.Update(obj);

            return await SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(IEnumerable<long> Ids)
        {
            _dataContext
                .SpotScale
                .RemoveRange(
                    _dataContext
                    .SpotScale
                    .Where(x => Ids.Contains(x.Id))
                );

            return await SaveChangesAsync();
        }

        #endregion

        public Task<string> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }
    }
}