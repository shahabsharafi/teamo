using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text;
using Teamo.Assets.Email;
using Teamo.Assets.SMS;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Domain
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string TokenProvider { get; set; } = IdentitySettings.TOKEN_PROVIDER_NONE;
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
        private readonly IMapper _mapper;
        private readonly IOptions<IdentitySettings> _options;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginHandler(
            ILogger<LoginHandler> logger,
            IMapper mapper,
            IOptions<IdentitySettings> options,
            IEmailService emailService,
            ISMSService smsService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _mapper = mapper;
            _options = options;
            _emailService = emailService;
            _smsService = smsService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName.Trim());
            if (user == null || user.IsDeleted)
                throw new Exception(Constants.Message.USER_NOT_FOUND);

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now.ToUniversalTime())
                throw new Exception(Constants.Message.USER_ISUNAUTHORIZED);

            
            
            if (user.TwoFactorEnabled)
            {
                if (string.IsNullOrEmpty(request.VerificationCode))
                {
                    await SendVerificationCode(user, _options.Value.TokenProvider, cancellationToken);
                    return new LoginDto()
                    {
                        TokenProvider = _options.Value.TokenProvider
                    };
                }
                else
                {
                    bool verified = await _userManager.VerifyTwoFactorTokenAsync(user, _options.Value.TokenProvider, request.VerificationCode);
                    if (!verified)
                    {
                        await UnsuccessfulAuthentication(user);
                        throw new Exception(Constants.Message.VERIFIY_CODE_IS_NOT_VALID);
                    }
                }
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    await UnsuccessfulAuthentication(user);
                    throw new Exception(Constants.Message.LOGIN_FAILED);
                }
            }

            _logger.LogInformation($"{user.UserName} loged in!");

            var token = new LoginDto()
            {
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email
            };
            return token;
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
            if (tokenProvider == TokenOptions.DefaultEmailProvider)
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
