using Microsoft.AspNetCore.Mvc;
using Storage.Module.Localization;

namespace Storage.Module.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        public IActionResult StringToResult((bool isSuccess, string message) tuple)
        {
            if (tuple.isSuccess)
            {
                return Ok(new { Message = tuple.message });
            }
            else
            {
                return BadRequest(string.Format(StorageLoc.CanNotExecute, tuple.message));
            }
        }
    }
}