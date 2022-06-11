using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IFuturesScaleRepository
    {
        public IEnumerable<FuturesScale> Get();
        public IEnumerable<FuturesScale> GetSorted();
        public IEnumerable<FuturesScale> GetByUnique(long uniqueId);
        public Task<FuturesScale> GetByIdAsync(long Id);
        public Task<(bool IsSuccess, string Message)> CreateAsync(FuturesScale obj);
        public Task<(bool IsSuccess, string Message)> UpdateAsync(FuturesScale obj, FuturesScale newObj);
        public Task<(bool IsSuccess, string Message)> DeleteAsync(IEnumerable<long> Ids);
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
    }
}