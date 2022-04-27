using Microsoft.AspNetCore.Mvc;

namespace Teamo.Workspace.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly ILogger<StorageController> _logger;

        public StorageController(ILogger<StorageController> logger)
        {
            _logger = logger;
        }

        [HttpPut(Name = "CreateStorage")]
        public ActionResult CreateStorage()
        {
            return Ok();
        }

        [HttpPut(Name = "RenameStorageName")]
        public ActionResult RenameStorageName()
        {
            return Ok();
        }

        [HttpPut(Name = "RemoveStorage")]
        public ActionResult DeleteStorage()
        {
            return Ok();
        }        
    }
}