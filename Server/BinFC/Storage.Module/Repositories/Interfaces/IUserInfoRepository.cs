using Storage.Module.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IUserInfoRepository
    {
        public IEnumerable<UserInfo> Get();
        public IEnumerable<UserInfo> Get(DateTime beforeDate);
        public Task<UserInfo> GetByIdAsync(long Id);
        public List<long> GetChatIdByUserNickName(List<string> userNickNames);
        public Task<(bool IsSuccess, string Message)> CreateAsync(UserInfo obj);
        public Task<(bool IsSuccess, string Message)> UpdateAsync(UserInfo obj, UserInfo newObj);
        public Task<(bool IsSuccess, string Message)> DeleteAsync(UserInfo obj);
        public Task<(bool IsSuccess, string Message)> ApproveAsync(IEnumerable<long> ids);
        public Task<(bool IsSuccess, string Message)> NotApproveAsync(IEnumerable<long> ids);
        public Task<(bool IsSuccess, string Message)> ApproveAllAsync();
        public Task<(bool IsSuccess, string Message)> NotApproveAllAsync();
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
        public Task<UserInfo> GetUserInfoByChatIdAsync(long chatId, bool isNeedTracking = true);
        public Task<UserInfo> GetUserInfoByUserIdAsync(long userId, bool isNeedTracking = true);
    }
}