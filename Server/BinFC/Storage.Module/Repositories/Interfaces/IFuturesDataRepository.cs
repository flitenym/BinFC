using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IFuturesDataRepository
    {
        public IEnumerable<FuturesData> Get();
        public Task CreateAsync(Data obj);
        public Task<string> DeleteAsync(IEnumerable<long> Ids);
        public Task<string> DeleteAllAsync();
        public Task<string> SaveChangesAsync();
    }
}