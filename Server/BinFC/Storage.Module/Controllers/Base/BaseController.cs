using Microsoft.AspNetCore.Mvc;
using Storage.Module.Localization;

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
                return BadRequest(string.Format(StorageLoc.CanNotExecute, error));
            }
        }
    }
}
