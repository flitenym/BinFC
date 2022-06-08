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

        public IEnumerable<FuturesScale> GetSorted()
        {
            return _dataContext
                .FuturesScale
                .Include(i => i.Unique)
                .OrderBy(x => x.FromValue);
        }

        public Task<FuturesScale> GetByIdAsync(long id)
        {
            return _dataContext
                .FuturesScale
                .Include(i => i.Unique)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<(bool IsSuccess, string Message)> CreateAsync(FuturesScale obj)
        {
            _dataContext.FuturesScale.Add(obj);

            return SaveChangesAsync();
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(FuturesScale obj, FuturesScale newObj)
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

        public Task<(bool IsSuccess, string Message)> DeleteAsync(IEnumerable<long> ids)
        {
            _dataContext
                .FuturesScale
                .RemoveRange(
                    _dataContext
                    .FuturesScale
                    .Where(x => ids.Contains(x.Id))
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