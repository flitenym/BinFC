using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IPayHistoryRepository
    {
        public IEnumerable<PayHistory> Get();
        public Task<string> SaveChangesAsync();
    }
}
