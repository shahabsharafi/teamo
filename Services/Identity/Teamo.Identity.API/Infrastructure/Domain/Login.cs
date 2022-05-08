using MediatR;
using Microsoft.AspNetCore.Identity;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Domain
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool VerificationRequired { get; set; } = false;
    }
    public class LoginCommand : IRequest<LoginDto>
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public bool IsTwoFactor { get; set; }
        public string VerificationCode { get; set; } = string.Empty;

    }

    public class LoginHandler : IRequestHandler<LoginCommand, LoginDto>
    {
        private readonly ILogger<LoginHandler> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginHandler(
            ILogger<LoginHandler> logger,
            IAuthenticationService authenticationService,
            IVerificationCodeService verificationCodeService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _verificationCodeService = verificationCodeService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// <para>Handle login proccess</para>
        /// <br>1- find user by email or username</br> 
        /// <br>2- check if user exists and not deleted</br> 
        /// <br>3- check if user is locked</br> 
        /// <br>4- if two factor is enabled</br>
        /// <br>    4.1- then if verification code is not recieved send it</br>
        /// <br>    4.2- else if verification code is recieved check it</br>
        /// <br>5- sign in by fined users username and recived password</br>
        /// <br>6- return tocken includ (email, username, fullname)</br>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = null;

            if (!string.IsNullOrEmpty(request.Email))
                user = await _userManager.FindByEmailAsync(request.Email.Trim());
            else if (!string.IsNullOrEmpty(request.UserName))
                user = await _userManager.FindByNameAsync(request.UserName.Trim());

            if (user == null || user.IsDeleted)
                throw new Exception(Constants.Message.USER_NOT_FOUND);

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now.ToUniversalTime())
                throw new Exception(Constants.Message.USER_ISUNAUTHORIZED);



            if (user.TwoFactorEnabled)
            {
                if (string.IsNullOrEmpty(request.VerificationCode))
                {
                    await _verificationCodeService.SendCodeAsync(user);
                    return new LoginDto() { VerificationRequired = true };
                }
                else
                {
                    bool verified = await _verificationCodeService.VerifyCodeAsync(user, request.VerificationCode);
                    if (!verified)
                    {
                        await _authenticationService.UnsuccessfulAuthentication(user);
                        throw new Exception(Constants.Message.VERIFIY_CODE_IS_NOT_VALID);
                    }
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                await _authenticationService.UnsuccessfulAuthentication(user);
                throw new Exception(Constants.Message.LOGIN_FAILED);
            }

            _logger.LogInformation($"{user.UserName} loged in!");

            var token = new LoginDto()
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName
            };
            return token;
        }
    }
}
