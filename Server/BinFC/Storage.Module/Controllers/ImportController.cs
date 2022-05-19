using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Module.Controllers.Base;
using Storage.Module.Import.DTO;
using Storage.Module.Import.Services.Interfaces;
using System.Threading.Tasks;

namespace Storage.Module.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : BaseController
    {
        private readonly IImportService _importService;
        private readonly ILogger<ImportController> _logger;
        public ImportController(IImportService importService, ILogger<ImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromForm] ImportDTO model)
        {
            if (!model.IsValid())
            {
                return BadRequest("Ошибка проверки запроса.");
            }

            (bool IsSuccess, string Error) = 
                await _importService.ImportAsync(model.GetFileContent(model.File), model.File.FileName, model.ImportType);

            if (IsSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest(Error);
            }
        }
    }
}