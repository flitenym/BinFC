using BinanceApi.Module.Controllers.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinanceApi.Module.Services.Interfaces
{
    public interface IPaymentService
    {
        public Task<(IEnumerable<PaymentDTO> PaymentInfo, string Message)> CalculatePaymentInfoAsync();
        public Task<(bool IsSuccess, string Message)> BinancePayAsync(IEnumerable<PaymentDTO> paymentsInfo);
        public Task<string> GetBinanceBalanceAsync();
    }
}