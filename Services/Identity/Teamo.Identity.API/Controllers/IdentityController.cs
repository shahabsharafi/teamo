using MediatR;
using Microsoft.AspNetCore.Mvc;
using Teamo.Identity.API.Infrastructure.Domain;

namespace Teamo.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IdentityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("register-user")]
        public async Task<ActionResult> RegisterUser(
            RegisterUserCommand registerUserCommand, 
            CancellationToken cancellationToken)
        {
            await _mediator.Send(registerUserCommand, cancellationToken);
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginDto>> Login(
            LoginCommand loginCommand, 
            CancellationToken cancellationToken)
        {
            LoginDto dto = await _mediator.Send(loginCommand, cancellationToken);
            return Ok(dto);
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<ActionResult<ForgotPasswordDto>> ForgotPassword(
            ForgotPasswordCommand forgotPasswordCommand, 
            CancellationToken cancellationToken)
        {
            ForgotPasswordDto dto = await _mediator.Send(forgotPasswordCommand, cancellationToken);
            return Ok(dto);
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<ActionResult> ChangePassword(
            ChangePasswordCommand changePasswordCommand,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(changePasswordCommand, cancellationToken);
            return Ok();
        }
    }
}