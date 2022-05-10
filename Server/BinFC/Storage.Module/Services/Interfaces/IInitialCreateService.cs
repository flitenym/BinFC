using System.Threading.Tasks;

namespace Storage.Module.Services.Interfaces
{
    public interface IInitialCreateService
    {
        public Task InitialCreateValuesAsync();
    }
}