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

        public Task<Unique> GetByIdAsync(long id)
        {
            return _dataContext
                .Unique
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<(bool IsSuccess, string Message)> CreateAsync(Unique obj)
        {
            _dataContext.Unique.Add(obj);

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> UpdateAsync(Unique obj, Unique newObj)
        {
            obj.Name = newObj.Name;

            _dataContext.Unique.Update(obj);

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> DeleteAsync(Unique obj)
        {
            _dataContext.Unique.Remove(obj);

            return SaveChangesAsync();
        }

        #endregion

        public Task<(bool IsSuccess, string Message)> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }
    }
}