using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Domain
{
    public class RegisterUserCommand : IRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterUserHandler> _logger;
        private readonly IMapper _mapper;

        public RegisterUserHandler(
            ILogger<RegisterUserHandler> logger,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// <para>Handel RegisterUserCommand</para>
        /// <br>1- check user duplication</br> 
        /// <br>2- check email duplication</br> 
        /// <br>3- create user</br> 
        /// <br>4- log</br> 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken = default)
        {
            var duplicateUser = await _userManager.FindByNameAsync(request.UserName.Trim());
            if (duplicateUser != null)
                throw new Exception(Constants.Message.DUPLICATE_USER);

            var duplicateEmail = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (duplicateEmail != null)
                throw new Exception(Constants.Message.DUPLICATE_EMAIL);

            var user = _mapper.Map<ApplicationUser>(request);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new Exception(Constants.Message.REGISTER_FALED);

            _logger.LogInformation($"{user.UserName} pasword recoverd successfully");

            return Unit.Value;
        }
    }
}
