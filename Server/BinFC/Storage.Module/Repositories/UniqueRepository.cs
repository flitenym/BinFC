using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class UniqueRepository : IUniqueRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<UniqueRepository> _logger;
        public UniqueRepository(DataContext dataContext, IBaseRepository baseRepository, ILogger<UniqueRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _logger = logger;
        }

        #region Controller methods

        public IEnumerable<Unique> Get()
        {
            return _dataContext
                .Unique
                .OrderBy(x=>x.Id);
        }

        public async Task<Unique> GetByIdAsync(long Id)
        {
            return await _dataContext
                .Unique
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<string> CreateAsync(Unique obj)
        {
            _dataContext.Unique.Add(obj);

            return await _baseRepository.SaveChangesAsync();
        }

        public async Task<string> UpdateAsync(Unique obj, Unique newObj)
        {
            obj.Name = newObj.Name;

            _dataContext.Unique.Update(obj);

            return await _baseRepository.SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(Unique obj)
        {
            _dataContext.Unique.Remove(obj);

            return await _baseRepository.SaveChangesAsync();
        }

        #endregion

        public Task<string> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }
    }
}