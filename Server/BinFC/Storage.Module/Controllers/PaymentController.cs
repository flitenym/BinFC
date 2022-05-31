using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Controllers.DTO;
using Storage.Module.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> CalculatePaymentInfo()
        {
            (IEnumerable<PaymentDTO> paymentInfo, string message) = await _paymentService.CalculatePaymentInfoAsync();

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(message);
            }

            return Ok(paymentInfo);
        }

        [HttpPost]
        public async Task<IActionResult> Pay([FromBody] IEnumerable<PaymentDTO> objects)
        {
            return Ok();
        }
    }
}