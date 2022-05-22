using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<AdminRepository> _logger;
        public AdminRepository(DataContext dataContext, IBaseRepository baseRepository, ILogger<AdminRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _logger = logger;
        }

        #region Controller methods

        public IEnumerable<Admin> Get()
        {
            return _dataContext.Admins;
        }

        public async Task<Admin> GetByIdAsync(long Id)
        {
            return await _dataContext.Admins.FindAsync(Id);
        }

        public async Task<bool> LoginAsync(Admin obj)
        {
            return await _dataContext.Admins
                .AsNoTracking()
                .Where(x => x.UserName.Equals(obj.UserName))
                .Where(x => x.Password.Equals(obj.Password))
                .AnyAsync();
        }

        public async Task<string> CreateAsync(Admin obj)
        {
            _dataContext.Admins.Add(obj);

            return await SaveChangesAsync();
        }

        public async Task<string> UpdateAsync(Admin obj, Admin newObj)
        {
            obj.UserName = newObj.UserName;
            obj.Password = newObj.Password;

            _dataContext.Admins.Update(obj);

            return await SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(Admin obj)
        {
            _dataContext.Admins.Remove(obj);

            return await SaveChangesAsync();
        }

        public Task<string> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }

        #endregion
    }
}