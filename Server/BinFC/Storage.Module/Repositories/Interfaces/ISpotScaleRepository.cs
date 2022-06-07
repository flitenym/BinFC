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
        public Task<string> CreateAsync(SpotScale obj);
        public Task<string> UpdateAsync(SpotScale obj, SpotScale newObj);
        public Task<string> DeleteAsync(IEnumerable<long> Ids);
        public Task<string> SaveChangesAsync();
    }
}