using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Teamo.Identity.API.Infrastructure;
using Teamo.Identity.API.Infrastructure.Domain;
using Teamo.Identity.API.Infrastructure.Models;
using Xunit;

namespace Teamo.Identity.Test
{
    public class ForgotPasswordTest : BaseTestClass
    {
        private readonly Mock<ILogger<ForgotPasswordHandler>> _logger;
        private readonly Mock<IAuthenticationService> _authenticationService;
        private readonly Mock<IVerificationCodeService> _verificationCodeService;
        private readonly ForgotPasswordHandler _handler;
        public ForgotPasswordTest() : base()
        {            
            _logger = new Mock<ILogger<ForgotPasswordHandler>>();

            _authenticationService = new Mock<IAuthenticationService>();

            _verificationCodeService = new Mock<IVerificationCodeService>();

            _authenticationService.Setup(_ => _.UnsuccessfulAuthentication(It.IsAny<ApplicationUser>()));

            _userManager.Setup(_ => _.GeneratePasswordResetTokenAsync(EXISTS_USER));

            _userManager.Setup(_ => _.ResetPasswordAsync(EXISTS_USER, RESET_TOKEN, NEW_PASSWORD))
                .ReturnsAsync(() => IdentityResult.Success);

            _userManager.Setup(_ => _.ResetPasswordAsync(EXISTS_USER, WRONG_RESET_TOKEN, NEW_PASSWORD))
                .ReturnsAsync(() => IdentityResult.Failed());

            _verificationCodeService.Setup(_ => _.SendCodeAsync(EXISTS_USER));

            _verificationCodeService.Setup(_ => _.VerifyCodeAsync(EXISTS_USER, VERIFICATION_CODE))
                .ReturnsAsync(() => true);

            _verificationCodeService.Setup(_ => _.VerifyCodeAsync(EXISTS_USER, WRONG_VERIFICATION_CODE))
                .ReturnsAsync(() => false);

            _handler = new(
                _logger.Object, 
                _authenticationService.Object, 
                _verificationCodeService.Object, 
                _userManager.Object);
        }

        [Fact]
        public async void Forgot_Password_Send_Code()
        {
            ForgotPasswordCommand command = new()
            {
                UserNameOrEmail = EXISTS_USER_NAME
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _userManager.Verify(_ => _.FindByNameAsync(EXISTS_USER_NAME));

            _verificationCodeService.Verify(_ => _.SendCodeAsync(EXISTS_USER));

            Assert.Null(exception);
        }

        [Fact]
        public async void Forgot_Password_Verify_Code_And_Generate_Token()
        {
            ForgotPasswordCommand command = new()
            {
                UserNameOrEmail = EXISTS_USER_NAME,
                VerificationCode = VERIFICATION_CODE
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _verificationCodeService.Verify(_ => _.VerifyCodeAsync(EXISTS_USER, VERIFICATION_CODE));

            _userManager.Verify(_ => _.GeneratePasswordResetTokenAsync(EXISTS_USER));

            Assert.Null(exception);
        }

        [Fact]
        public async void Forgot_Password_If_Verificatio_Code_Is_Not_Verified()
        {
            ForgotPasswordCommand command = new()
            {
                UserNameOrEmail = EXISTS_USER_NAME,
                VerificationCode = WRONG_VERIFICATION_CODE
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            Assert.True(exception.Message == Constants.Message.VERIFIY_CODE_IS_NOT_VALID);
        }

        [Fact]
        public async void Forgot_Password_ResetPassword()
        {
            ForgotPasswordCommand command = new()
            {
                UserNameOrEmail = EXISTS_USER_NAME,
                ResetToken = RESET_TOKEN,
                NewPassword = NEW_PASSWORD
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _userManager.Verify(_ => _.ResetPasswordAsync(EXISTS_USER, RESET_TOKEN, NEW_PASSWORD));

            Assert.Null(exception);
        }

        [Fact]
        public async void Forgot_Password_If_Reset_Token_Is_Not_Verified()
        {
            ForgotPasswordCommand command = new()
            {
                UserNameOrEmail = EXISTS_USER_NAME,
                ResetToken = WRONG_RESET_TOKEN,
                NewPassword = NEW_PASSWORD
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            Assert.True(exception.Message == Constants.Message.FORGOT_PASSWORD_FALED);
        }
    }    
}
