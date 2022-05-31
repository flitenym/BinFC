using Storage.Module.Controllers.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Services.Interfaces
{
    public interface IPaymentService
    {
        public Task<(IEnumerable<PaymentDTO> PaymentInfo, string Message)> CalculatePaymentInfoAsync();
    }
}