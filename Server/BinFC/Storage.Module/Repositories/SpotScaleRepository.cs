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

        public IEnumerable<SpotScale> GetSorted()
        {
            return _dataContext
                .SpotScale
                .Include(i => i.Unique)
                .OrderBy(x => x.FromValue);
        }

        public Task<SpotScale> GetByIdAsync(long id)
        {
            return _dataContext
                .SpotScale
                .Include(i => i.Unique)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<(bool IsSuccess, string Message)> CreateAsync(SpotScale obj)
        {
            _dataContext.SpotScale.Add(obj);

            return SaveChangesAsync();
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(SpotScale obj, SpotScale newObj)
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

        public Task<(bool IsSuccess, string Message)> DeleteAsync(IEnumerable<long> Ids)
        {
            _dataContext
                .SpotScale
                .RemoveRange(
                    _dataContext
                    .SpotScale
                    .Where(x => Ids.Contains(x.Id))
                );

            return SaveChangesAsync();
        }

        #endregion

        public Task<(bool IsSuccess, string Message)> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }
    }
}