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
        public Task<(bool IsSuccess, string Message)> CreateAsync(Unique obj);
        public Task<(bool IsSuccess, string Message)> UpdateAsync(Unique obj, Unique newObj);
        public Task<(bool IsSuccess, string Message)> DeleteAsync(Unique obj);
        public Task<(bool IsSuccess, string Message)> SaveChangesAsync();
    }
}
