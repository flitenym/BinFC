using Storage.Module.Entities;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IBaseRepository
    {
        public Task<string> SaveChangesAsync();
        public Task<UserInfo> GetOrCreateUserInfoAsync(long userId);
        public Task<Unique> GetDefaultUniqueAsync();
    }
}
