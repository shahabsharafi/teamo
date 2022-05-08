using MediatR;
using Microsoft.AspNetCore.Identity;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Domain
{
    public class ForgotPasswordDto
    {        
        public string UserName { get; set; } = string.Empty;
        public bool VerificationRequired { get; set; } = false;  
        public string VerificationCode { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
    }
    public class ForgotPasswordCommand : IRequest<ForgotPasswordDto>
    {
        public string UserNameOrEmail { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;

    }

    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordDto>
    {
        private readonly ILogger<ForgotPasswordHandler> _logger;

        private readonly IVerificationCodeService _verificationCodeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationService _authenticationService;
        public ForgotPasswordHandler(
            ILogger<ForgotPasswordHandler> logger,
            IAuthenticationService authenticationService,
            IVerificationCodeService verificationCodeService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _verificationCodeService = verificationCodeService;
            _userManager = userManager;
        }

        /// <summary>
        /// <para>Handle forgot password proccess</para>
        /// <br>1- try to find user by email than by username</br> 
        /// <br>2- check if user exists and not deleted</br> 
        /// <br>3- check if user is locked</br> 
        /// <br>4- check:</br>
        /// <br>    4.1- if verification code is recieved check it and generate rest token</br>
        /// <br>    4.2- if reset token is recieved reset password by reset token and check success</br>
        /// <br>    4.3- else send verification code</br>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ForgotPasswordDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.UserNameOrEmail.Trim());

            if (user == null)
                user = await _userManager.FindByNameAsync(request.UserNameOrEmail.Trim());

            if (user == null || user.IsDeleted)
                throw new Exception(Constants.Message.USER_NOT_FOUND);

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now.ToUniversalTime())
                throw new Exception(Constants.Message.USER_ISUNAUTHORIZED);

            ForgotPasswordDto dto;
            if (!string.IsNullOrEmpty(request.VerificationCode))
            {
                bool verified = await _verificationCodeService.VerifyCodeAsync(user, request.VerificationCode);
                
                if (!verified)
                {
                    await _authenticationService.UnsuccessfulAuthentication(user);
                    throw new Exception(Constants.Message.VERIFIY_CODE_IS_NOT_VALID);
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                dto = new ForgotPasswordDto()
                {
                    ResetToken = resetToken
                };
            }
            else if (!string.IsNullOrEmpty(request.ResetToken))
            {
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);
                
                if (!resetPasswordResult.Succeeded)
                    throw new Exception(Constants.Message.FORGOT_PASSWORD_FALED);

                dto = new ForgotPasswordDto();
            }
            else
            {
                await _verificationCodeService.SendCodeAsync(user);
                
                dto = new ForgotPasswordDto()
                {
                    UserName = user.UserName,
                    VerificationRequired = true
                };
            }                

            _logger.LogInformation($"{user.UserName} pasword recoverd successfully");

            return dto;
        }
    }
}
