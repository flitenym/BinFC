using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ISpotScaleRepository
    {
        public IEnumerable<SpotScale> Get();
        public IEnumerable<SpotScale> GetSorted();
        public Task<SpotScale> GetByIdAsync(long Id);
        public Task<(bool IsSuccess, string Message)> CreateAsync(SpotScale obj);
        public Task<(bool IsSuccess, string Message)> UpdateAsync(SpotScale obj, SpotScale newObj);
        public Task<(bool IsSuccess, string Message)> DeleteAsync(IEnumerable<long> Ids);
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
    }
}