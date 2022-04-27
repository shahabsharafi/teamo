using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Teamo.Assets.Email;
using Teamo.Assets.SMS;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Domain
{
    public class ForgotPasswordDto
    {        
        public string UserName { get; set; } = string.Empty;
        public string TokenProvider { get; set; } = IdentitySettings.TOKEN_PROVIDER_NONE;  
        public string VerificationCode { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
    }
    public class ForgotPasswordCommand : IRequest<ForgotPasswordDto>
    {
        public string UserNameOrEmail { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;

    }

    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordDto>
    {
        private readonly ILogger<ForgotPasswordHandler> _logger;
        private readonly IOptions<IdentitySettings> _options;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ForgotPasswordHandler(
            ILogger<ForgotPasswordHandler> logger,
            IOptions<IdentitySettings> options,
            IEmailService emailService,
            IMemoryCache memoryCache,
            ISMSService smsService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _options = options;
            _memoryCache = memoryCache;
            _emailService = emailService;
            _smsService = smsService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<ForgotPasswordDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.UserNameOrEmail.Trim());

            if (user == null)
                user = await _userManager.FindByNameAsync(request.UserNameOrEmail.Trim());

            if (user == null || user.IsDeleted)
                throw new Exception(Constants.Message.USER_NOT_FOUND);

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now.ToUniversalTime())
                throw new Exception(Constants.Message.USER_ISUNAUTHORIZED);
            
            var userVerificationCodeKey = user.UserName + "_verfication_code";
            if (string.IsNullOrEmpty(request.VerificationCode))
            {
                await SendVerificationCode(user, _options.Value.TokenProvider, cancellationToken);
                return new ForgotPasswordDto()
                {
                    UserName = user.UserName,
                    TokenProvider = _options.Value.TokenProvider
                };
            }
            else if (!string.IsNullOrEmpty(request.VerificationCode))
            {
                if (string.IsNullOrEmpty(request.NewPassword) && string.IsNullOrEmpty(request.ConfirmNewPassword))
                {
                    bool verified = await _userManager.VerifyTwoFactorTokenAsync(user, _options.Value.TokenProvider, request.VerificationCode);
                    if (!verified)
                    {
                        await UnsuccessfulAuthentication(user);
                        throw new Exception(Constants.Message.VERIFIY_CODE_IS_NOT_VALID);
                    }

                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    return new ForgotPasswordDto()
                    {
                        ResetToken = resetToken
                    };
                }
                else if (!string.IsNullOrEmpty(request.NewPassword) && !string.IsNullOrEmpty(request.ConfirmNewPassword))
                {                    
                    var resetPasswordResult = await _userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);
                    if (!resetPasswordResult.Succeeded)
                    {
                        throw new Exception(Constants.Message.FORGOT_PASSWORD_FALED);
                    }
                }
            }

            _logger.LogInformation($"{user.UserName} pasword recoverd successfully");

            return new ForgotPasswordDto();
        }

        private async Task UnsuccessfulAuthentication(ApplicationUser user)
        {
            if (user.AccessFailedCount > 2)
            {
                user.LockoutEnd = DateTime.Now.AddMinutes(2).ToUniversalTime();
                user.AccessFailedCount = 0;
                await _userManager.UpdateAsync(user);

                throw new Exception(Constants.Message.USER_LOCKED);
            }
            else
            {
                user.AccessFailedCount += 1;
                await _userManager.UpdateAsync(user);                
            }
        }

        private async Task SendVerificationCode(ApplicationUser user, string tokenProvider, CancellationToken cancellationToken)
        {
            if (user == null || string.IsNullOrEmpty(user.FullName) || string.IsNullOrEmpty(user.Email))
                throw new ArgumentNullException();

            var userVerificationCodeKey = user.UserName + "_verfication_code";
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
            var msg = $"Use {token} to verify your login";

            if (tokenProvider == TokenOptions.DefaultPhoneProvider)
            { 
                var sms = new SMS()
                {
                    SenderNumber = "",
                    ReciverNumber = new string[] { user.PhoneNumber },
                    Message = msg
                };
                await _smsService.SendAsync(sms);
            }
            else if (tokenProvider == TokenOptions.DefaultEmailProvider)
            {
                var userEmailAddress = new List<EmailAddress>(){
                    new EmailAddress() { Name = user.FullName, Address = user.Email }
                };

                var email = new EmailMessage()
                {
                    FromAddresses = null,
                    ToAddresses = userEmailAddress,
                    Subject = "Verification code",
                    Content = msg
                };
                await _emailService.SendAsync(email);
            }
        }
    }
}
