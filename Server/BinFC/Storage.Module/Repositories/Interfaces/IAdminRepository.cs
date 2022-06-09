using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        public IEnumerable<Admin> Get();
        public Task<Admin> GetByIdAsync(long Id);
        public Task<bool> LoginAsync(Admin obj);
        public Task<(bool IsSuccess, string Message)> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
        public Task<(bool IsSuccess, string Message)> UpdateLanguageAsync(string userName, string language);
        public Task<(bool IsSuccess, string Message)> GetLanguageAsync(string userName);
        public Task<(bool IsSuccess, string Message)> CreateAsync(Admin obj);
        public Task<(bool IsSuccess, string Message)> UpdateAsync(Admin obj, Admin newObj);
        public Task<(bool IsSuccess, string Message)> DeleteAsync(Admin obj);
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
    }
}