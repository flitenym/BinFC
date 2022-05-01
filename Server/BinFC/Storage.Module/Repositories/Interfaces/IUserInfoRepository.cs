using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IUserInfoRepository
    {
        public IEnumerable<UserInfo> Get();
        public Task<UserInfo> GetByIdAsync(long Id);
        public Task<string> CreateAsync(UserInfo obj);
        public Task<string> UpdateAsync(UserInfo obj, UserInfo newObj);
        public Task<string> DeleteAsync(UserInfo obj);
        public Task<string> SaveChangesAsync();
        public Task<UserInfo> GetUserInfoByChatIdAsync(long chatId, bool isNeedTracking = true);
        public Task<UserInfo> GetUserInfoByUserIdAsync(long userId, bool isNeedTracking = true);
    }
}
