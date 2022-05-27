using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PayHistoryController : BaseController
    {
        private readonly IPayHistoryRepository _payHistoryRepository;
        private readonly ILogger<PayHistoryController> _logger;
        public PayHistoryController(IPayHistoryRepository payHistoryRepository, ILogger<PayHistoryController> logger)
        {
            _payHistoryRepository = payHistoryRepository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<PayHistory> Get()
        {
            return _payHistoryRepository.Get();
        }
    }
}