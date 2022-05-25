using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface IUniqueRepository
    {
        public IEnumerable<Unique> Get();
        public Task<Unique> GetByIdAsync(long Id);
        public Task<string> CreateAsync(Unique obj);
        public Task<string> UpdateAsync(Unique obj, Unique newObj);
        public Task<string> DeleteAsync(Unique obj);
        public Task<string> SaveChangesAsync();
    }
}
