using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Export.DTO;
using Storage.Module.Export.Services.Interfaces;
using Storage.Module.Import.DTO;
using Storage.Module.Import.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : BaseController
    {
        private readonly IExportPayHistoryService _exportService;
        private readonly ILogger<ExportController> _logger;
        public ExportController(IExportPayHistoryService exportService, ILogger<ExportController> logger)
        {
            _exportService = exportService;
            _logger = logger;
        }

        [HttpPost("payhistory")]
        public async Task<IActionResult> Export([FromBody] ExportPayHistoryDTO model)
        {
            if (model == null || model.Ids == null)
            {
                return BadRequest("Отправлена пустая сущность.");
            }

            if (!model.IsValid())
            {
                return BadRequest("Ошибка проверки запроса.");
            }

            (bool isSuccess, string error, byte[] fileContent) = 
                await _exportService.ExportAsync(model.Ids);

            if (isSuccess)
            {
                return new FileContentResult(fileContent, "text/csv");
            }
            else
            {
                return BadRequest(error);
            }
        }
    }
}