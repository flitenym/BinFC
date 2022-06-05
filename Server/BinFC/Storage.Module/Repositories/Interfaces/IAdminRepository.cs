using Microsoft.Extensions.Logging;
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
        public Task<string> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
        public Task<string> UpdateLanguageAsync(string userName, string language);
        public Task<string> CreateAsync(Admin obj);
        public Task<string> UpdateAsync(Admin obj, Admin newObj);
        public Task<string> DeleteAsync(Admin obj);
        public Task<string> SaveChangesAsync();
    }
}