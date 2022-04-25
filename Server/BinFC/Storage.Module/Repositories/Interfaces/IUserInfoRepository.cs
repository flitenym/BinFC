using Storage.Module.Entities;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IUserInfoRepository
    {
        public Task DataContextSaveChanges();
        public Task<UserInfo> GetUserInfoByChatId(long chatId, bool isNeedTracking = true);
        public Task<UserInfo> GetUserInfoByUserId(long userId, bool isNeedTracking = true);
    }
}
