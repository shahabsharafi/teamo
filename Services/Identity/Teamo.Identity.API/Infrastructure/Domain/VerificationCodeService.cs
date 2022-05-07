using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Teamo.Assets.Email;
using Teamo.Assets.SMS;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Domain
{
    public interface IVerificationCodeService
    {
        Task SendCodeAsync(ApplicationUser user);
        Task<bool> VerifyCodeAsync(ApplicationUser user, string code);
    }
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly IOptions<IdentitySettings> _options;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public VerificationCodeService(
            IOptions<IdentitySettings> options,
            IEmailService emailService,
            ISMSService smsService,
            UserManager<ApplicationUser> userManager)
        {
            _options = options;
            _emailService = emailService;
            _smsService = smsService;
            _userManager = userManager;
        }

        public async Task SendCodeAsync(ApplicationUser user)
        {
            string tokenProvider = _options.Value.TokenProvider;
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
        public async Task<bool> VerifyCodeAsync(ApplicationUser user, string code)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, _options.Value.TokenProvider, code);
        }
    }
}
