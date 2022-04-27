using Microsoft.AspNetCore.Mvc;

namespace Teamo.Workspace.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkspaceController : ControllerBase
    {
        private readonly ILogger<WorkspaceController> _logger;

        public WorkspaceController(ILogger<WorkspaceController> logger)
        {
            _logger = logger;
        }

        [HttpPut(Name = "CreateTenant")]
        public ActionResult CreateTenant()
        {
            return Ok();
        }

        [HttpPut(Name = "ModifyTenant")]
        public ActionResult ModifyTenant()
        {
            return Ok();
        }

        [HttpPut(Name = "RemoveTenant")]
        public ActionResult RemoveTenant()
        {
            return Ok();
        }
    }
}