using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Teamo.Assets.Email;
using Teamo.Assets.SMS;
using Teamo.Identity.API.Infrastructure;
using Teamo.Identity.API.Infrastructure.Domain;
using Teamo.Identity.API.Infrastructure.Models;
using Xunit;

namespace Teamo.Identity.Test
{
    public class VerificationCodeServiceTest : BaseTestClass
    {
        private readonly Mock<IOptions<IdentitySettings>> _options;
        private readonly Mock<IEmailService> _emailService;
        private readonly Mock<ISMSService> _smsService;
        private readonly VerificationCodeService _verificationCodeService;
        public VerificationCodeServiceTest()
        {
            _options = new Mock<IOptions<IdentitySettings>>();

            _emailService = new Mock<IEmailService>();

            _smsService = new Mock<ISMSService>();

            _smsService.Setup(s => s.SendAsync(It.IsAny<SMS>()));

            _emailService.Setup(s => s.SendAsync(It.IsAny<EmailMessage>()));

            _verificationCodeService = new(
                _options.Object,
                _emailService.Object,
                _smsService.Object,
                _userManager.Object);
        }

        [Fact]
        public async void SendCode_SMS()
        {
            _options.Setup(o => o.Value).Returns(new IdentitySettings()
            {
                TokenProvider = TokenOptions.DefaultPhoneProvider
            });

            ApplicationUser user = new()
            {
                FullName = FULL_NAME,
                PhoneNumber = PHONE_NUMBER
            };

            Exception exception = 
                await Record.ExceptionAsync(async () => await _verificationCodeService.SendCodeAsync(user));

            _smsService.Verify(s => s.SendAsync(It.IsAny<SMS>()));

            Assert.Null(exception);
        }

        [Fact]
        public async void SendCode_Email()
        {
            _options.Setup(o => o.Value).Returns(new IdentitySettings()
            {
                TokenProvider = TokenOptions.DefaultEmailProvider
            });

            ApplicationUser user = new()
            {
                FullName = FULL_NAME,
                Email = EXISTS_EMAIL
            };

            Exception exception = 
                await Record.ExceptionAsync(async () => await _verificationCodeService.SendCodeAsync(user));

            _emailService.Verify(s => s.SendAsync(It.IsAny<EmailMessage>()));

            Assert.Null(exception);
        }
    }
}
