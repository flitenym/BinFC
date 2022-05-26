using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class FuturesScaleRepository : IFuturesScaleRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly IUniqueRepository _uniqueRepository;
        private readonly ILogger<FuturesScaleRepository> _logger;
        public FuturesScaleRepository(
            DataContext dataContext,
            IBaseRepository baseRepository,
            IUniqueRepository uniqueRepository,
            ILogger<FuturesScaleRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _uniqueRepository = uniqueRepository;
            _logger = logger;
        }

        #region Controller methods

        public IEnumerable<FuturesScale> Get()
        {
            return _dataContext
                .FuturesScale
                .Include(i => i.Unique)
                .OrderBy(x => x.Id);
        }

        public async Task<FuturesScale> GetByIdAsync(long Id)
        {
            return await _dataContext
                .FuturesScale
                .Include(i => i.Unique)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<string> CreateAsync(FuturesScale obj)
        {
            _dataContext.FuturesScale.Add(obj);

            return await SaveChangesAsync();
        }

        public async Task<string> UpdateAsync(FuturesScale obj, FuturesScale newObj)
        {
            obj.FromValue = newObj.FromValue;
            obj.Percent = newObj.Percent;

            var unique = await _uniqueRepository.GetByIdAsync(newObj.UniqueId);

            if (unique != null)
            {
                obj.UniqueId = newObj.UniqueId;
            }

            _dataContext.FuturesScale.Update(obj);

            return await SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(IEnumerable<long> Ids)
        {
            _dataContext
                .FuturesScale
                .RemoveRange(
                    _dataContext
                    .FuturesScale
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