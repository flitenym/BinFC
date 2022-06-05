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

        public async Task<string> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            Admin admin = await _dataContext
                .Admins
                .Where(x => x.UserName.Equals(userName))
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return $"Не найден пользователь с логином {userName}";
            }

            if (admin.Password.Equals(oldPassword))
            {
                if (oldPassword.Equals(newPassword))
                {
                    return "Старый и новый пароль не должны быть одинаковыми.";
                }
                else
                {
                    admin.Password = newPassword;

                    _dataContext.Admins.Update(admin);
                    return await _baseRepository.SaveChangesAsync();
                }
            }
            else
            {
                return $"Старый пароль указан неверно.";
            }
        }

        public async Task<string> UpdateLanguageAsync(string userName, string language)
        {
            Admin admin = await _dataContext
                .Admins
                .Where(x => x.UserName.Equals(userName))
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return $"Не найден пользователь с логином {userName}";
            }

            admin.Language = language;

            _dataContext.Admins.Update(admin);

            return await SaveChangesAsync();
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
            obj.Language = newObj.Language;

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