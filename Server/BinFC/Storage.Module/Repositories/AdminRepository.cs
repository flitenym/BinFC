using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Localization;
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
            return _dataContext
                .Admins
                .OrderBy(x => x.Id);
        }

        public async Task<Admin> GetByIdAsync(long id)
        {
            return await _dataContext.Admins.FindAsync(id);
        }

        public async Task<bool> LoginAsync(Admin obj)
        {
            return await _dataContext.Admins
                .AsNoTracking()
                .Where(x => x.UserName.Equals(obj.UserName))
                .Where(x => x.Password.Equals(obj.Password))
                .AnyAsync();
        }

        public async Task<(bool IsSuccess, string Message)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            Admin admin = await _dataContext
                .Admins
                .Where(x => x.UserName.Equals(userName))
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return (false, string.Format(StorageLoc.NotFoundUserByLogin, userName));
            }

            if (admin.Password.Equals(oldPassword))
            {
                if (oldPassword.Equals(newPassword))
                {
                    return (false, StorageLoc.OldAndNewPasswordShouldNotEqual);
                }
                else
                {
                    admin.Password = newPassword;

                    _dataContext.Admins.Update(admin);
                    return await SaveChangesAsync();
                }
            }
            else
            {
                return (false, StorageLoc.OldPasswordIncorrect);
            }
        }

        public async Task<(bool IsSuccess, string Message)> UpdateLanguageAsync(string userName, string language)
        {
            Admin admin = await _dataContext
                .Admins
                .Where(x => x.UserName.Equals(userName))
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return (false, string.Format(StorageLoc.NotFoundUserByLogin, userName));
            }

            admin.Language = language;

            _dataContext.Admins.Update(admin);

            return await SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> CreateAsync(Admin obj)
        {
            _dataContext.Admins.Add(obj);

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> UpdateAsync(Admin obj, Admin newObj)
        {
            obj.UserName = newObj.UserName;
            obj.Password = newObj.Password;
            obj.Language = newObj.Language;

            _dataContext.Admins.Update(obj);

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> DeleteAsync(Admin obj)
        {
            _dataContext.Admins.Remove(obj);

            return SaveChangesAsync();
        }

        public Task<(bool IsSuccess, string Message)> SaveChangesAsync()
        {
            return _baseRepository.SaveChangesAsync();
        }

        #endregion
    }
}