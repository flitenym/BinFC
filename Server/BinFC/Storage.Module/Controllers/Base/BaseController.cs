using Microsoft.AspNetCore.Mvc;

namespace Storage.Module.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        public IActionResult StringToResult(string error)
        {
            if (error == null)
            {
                return Ok();
            }
            else
            {
                return BadRequest($"Не удалось выполнить: {error}");
            }
        }
    }
}
