using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IFuturesScaleRepository
    {
        public IEnumerable<FuturesScale> Get();
        public IEnumerable<FuturesScale> GetSorted();
        public Task<FuturesScale> GetByIdAsync(long Id);
        public Task<string> CreateAsync(FuturesScale obj);
        public Task<string> UpdateAsync(FuturesScale obj, FuturesScale newObj);
        public Task<string> DeleteAsync(IEnumerable<long> Ids);
        public Task<string> SaveChangesAsync();
    }
}