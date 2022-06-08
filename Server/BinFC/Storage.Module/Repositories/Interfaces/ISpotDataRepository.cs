using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ISpotDataRepository
    {
        public IEnumerable<SpotData> Get();
        public IEnumerable<SpotData> GetLastData();
        public Task CreateAsync(Data obj);
        public Task<(bool IsSuccess, string Message)> DeleteAsync(IEnumerable<long> Ids);
        public Task<(bool IsSuccess, string Message)> DeleteAllAsync();
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
        public Task UpdateIsPaidByUserIdAsync(long userId);
    }
}
