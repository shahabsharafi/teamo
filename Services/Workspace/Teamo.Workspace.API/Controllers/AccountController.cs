using Microsoft.AspNetCore.Mvc;

namespace Teamo.Workspace.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<WorkspaceController> _logger;

        public AccountController(ILogger<WorkspaceController> logger)
        {
            _logger = logger;
        }

        [HttpPut(Name = "RegisterUser")]
        public ActionResult Register()
        {
            return Ok();
        }

        [HttpPut(Name = "Login")]
        public ActionResult Login()
        {
            return Ok();
        }

        [HttpPut(Name = "ChangePassword")]
        public ActionResult ChangePassword()
        {
            return Ok();
        }
    }
}