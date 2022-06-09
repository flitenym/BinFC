using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using BinanceApi.Module.Controllers.DTO;
using BinanceApi.Module.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BinanceApi.Module.Controllers
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
            (IEnumerable<PaymentDTO> paymentInfo, bool isSuccess, string message) = await _paymentService.CalculatePaymentInfoAsync();

            if (!isSuccess)
            {
                return BadRequest(message);
            }

            return Ok(paymentInfo);
        }

        [HttpGet("balance")]
        public async Task<ActionResult> GetBinanceBalance()
        {
            string message = await _paymentService.GetBinanceBalanceAsync();
            return Ok(message);
        }

        [HttpPost]
        public async Task<IActionResult> PayAsync([FromBody] IEnumerable<PaymentDTO> objects)
        {
            (bool isSuccess, string message) = await _paymentService.BinancePayAsync(objects);

            message = message.Trim();

            if (isSuccess)
            {
                return Ok(new { Message = message });
            }
            else
            {
                _logger.LogError(message);
                return BadRequest(message);
            }
        }
    }
}