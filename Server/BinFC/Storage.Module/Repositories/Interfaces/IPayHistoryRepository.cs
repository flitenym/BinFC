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
        public Task<string> DeleteAllAsync();
        public Task<string> CreateAsync(PayHistory obj);
        public Task<string> SaveChangesAsync();
    }
}
