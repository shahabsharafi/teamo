using MediatR;

namespace Teamo.Workspace.API.Infrastructure.Domain.Account
{
    public class RegisterUserCommand : IRequest
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand>
    {
        private readonly ILogger<RegisterUserHandler> _logger;

        public RegisterUserHandler(ILogger<RegisterUserHandler> logger)
        {
            _logger = logger;
        }
        public async Task<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            //check duplicate user
            //return Unit.Value;
            throw new NotImplementedException();
        }
    }
}
