using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IPayHistoryRepository
    {
        public IEnumerable<PayHistory> Get();
        public IEnumerable<PayHistory> Get(long[] ids);
        public Task<int> GetLastNumberPayAsync();
        public Task<(bool IsSuccess, string Message)> DeleteAllAsync();
        public Task<(bool IsSuccess, string Message)> CreateAsync(PayHistory obj);
        public Task<(bool IsSuccess, string Message)> CreateAsync(PayHistory obj, long userId);
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
    }
}
