using MediatR;
using Microsoft.AspNetCore.Mvc;
using Teamo.Identity.API.Infrastructure.Domain;

namespace Teamo.Identity.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IdentityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut(Name = "RegisterUser")]
        public async Task<ActionResult> RegisterUser(
            RegisterUserCommand registerUserCommand, 
            CancellationToken cancellationToken)
        {
            await _mediator.Send(registerUserCommand, cancellationToken);
            return Ok();
        }

        [HttpPut(Name = "Login")]
        public async Task<ActionResult<LoginDto>> Login(
            LoginCommand loginCommand, 
            CancellationToken cancellationToken)
        {
            LoginDto dto = await _mediator.Send(loginCommand, cancellationToken);
            return Ok(dto);
        }

        [HttpPut(Name = "ForgotPassword")]
        public async Task<ActionResult<ForgotPasswordDto>> ForgotPassword(
            ForgotPasswordCommand forgotPasswordCommand, 
            CancellationToken cancellationToken)
        {
            ForgotPasswordDto dto = await _mediator.Send(forgotPasswordCommand, cancellationToken);
            return Ok(dto);
        }

        [HttpPut(Name = "ChangePassword")]
        public async Task<ActionResult> ChangePassword(
            ChangePasswordCommand changePasswordCommand,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(changePasswordCommand, cancellationToken);
            return Ok();
        }
    }
}