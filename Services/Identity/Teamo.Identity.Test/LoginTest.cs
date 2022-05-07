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
    public class LoginTest : BaseTestClass
    {
        private readonly Mock<ILogger<LoginHandler>> _logger;
        private readonly Mock<IAuthenticationService> _authenticationService;
        private readonly Mock<IVerificationCodeService> _verificationCodeService;
        private readonly LoginHandler _handler;
        public LoginTest() : base()
        {            
            _logger = new Mock<ILogger<LoginHandler>>();

            _authenticationService = new Mock<IAuthenticationService>();

            _verificationCodeService = new Mock<IVerificationCodeService>();

            _authenticationService.Setup(_ => _.UnsuccessfulAuthentication(It.IsAny<ApplicationUser>()));

            _signInManager.Setup(_ => _.PasswordSignInAsync(It.IsIn(EXISTS_USER, TWO_FACTOR_ENABLED_USER), PASSWORD, false, false))
                .ReturnsAsync(() => SignInResult.Success);

            _signInManager.Setup(_ => _.PasswordSignInAsync(It.IsIn(EXISTS_USER, TWO_FACTOR_ENABLED_USER), WRONG_PASSWORD, false, false))
                .ReturnsAsync(() => SignInResult.Failed);

            _verificationCodeService.Setup(_ => _.SendCodeAsync(TWO_FACTOR_ENABLED_USER));

            _verificationCodeService.Setup(_ => _.VerifyCodeAsync(TWO_FACTOR_ENABLED_USER, VERIFICATION_CODE))
                .ReturnsAsync(() => true);

            _handler = new(
                _logger.Object, 
                _authenticationService.Object, 
                _verificationCodeService.Object, 
                _userManager.Object, 
                _signInManager.Object);
        }

        [Fact]
        public async void Login_By_Email()
        {
            LoginCommand command = new()
            {
                Email = EXISTS_EMAIL,
                Password = PASSWORD,
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _signInManager.Verify(_ => _.PasswordSignInAsync(EXISTS_USER, PASSWORD, false, false));

            Assert.Null(exception);
        }

        [Fact]
        public async void Login_By_Password()
        {
            LoginCommand command = new()
            {
                UserName = EXISTS_USER_NAME,
                Password = PASSWORD,                
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _signInManager.Verify(_ => _.PasswordSignInAsync(It.IsAny<ApplicationUser>(), PASSWORD, It.IsAny<bool>(), false));

            Assert.Null(exception);
        }

        [Fact]
        public async void Login_Failed()
        {
            LoginCommand command = new()
            {
                UserName = EXISTS_USER_NAME,
                Password = WRONG_PASSWORD,
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _authenticationService.Verify(_ => _.UnsuccessfulAuthentication(It.IsAny<ApplicationUser>()));

            Assert.True(exception.Message == Constants.Message.LOGIN_FAILED);
        }

        [Fact]
        public async void Login_If_User_Not_Found()
        {
            LoginCommand command = new()
            {
                UserName = NEW_USER_NAME,
                Password = PASSWORD,
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            Assert.True(exception.Message == Constants.Message.USER_NOT_FOUND);
        }

        [Fact]
        public async void Login_If_User_Is_Deleted()
        {
            LoginCommand command = new()
            {
                UserName = DELETED_USER_NAME,
                Password = PASSWORD,
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            Assert.True(exception.Message == Constants.Message.USER_NOT_FOUND);
        }

        [Fact]
        public async void Login_If_TwoFactorEnabled_Send()
        {
            LoginCommand command = new()
            {
                UserName = TWO_FACTOR_ENABLED_USER_NAME,
                Password = PASSWORD
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _verificationCodeService.Verify(_ => _.SendCodeAsync(TWO_FACTOR_ENABLED_USER));

            Assert.Null(exception);
        }

        [Fact]
        public async void Login_If_TwoFactorEnabled_Verified()
        {
            LoginCommand command = new()
            {
                UserName = TWO_FACTOR_ENABLED_USER_NAME,
                Password = PASSWORD,
                VerificationCode = VERIFICATION_CODE
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _verificationCodeService.Verify(_ => _.VerifyCodeAsync(TWO_FACTOR_ENABLED_USER, VERIFICATION_CODE));

            _signInManager.Verify(_ => _.PasswordSignInAsync(It.IsAny<ApplicationUser>(), PASSWORD, It.IsAny<bool>(), false));

            Assert.Null(exception);
        }

        [Fact]
        public async void Login_If_TwoFactorEnabled_Not_Verified()
        {
            LoginCommand command = new()
            {
                UserName = TWO_FACTOR_ENABLED_USER_NAME,
                Password = PASSWORD,
                VerificationCode = WRONG_VERIFICATION_CODE
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command, cancellationToken: default));

            _verificationCodeService.Verify(_ => _.VerifyCodeAsync(TWO_FACTOR_ENABLED_USER, WRONG_VERIFICATION_CODE));

            _authenticationService.Verify(_ => _.UnsuccessfulAuthentication(It.IsAny<ApplicationUser>()));

            Assert.True(exception.Message == Constants.Message.VERIFIY_CODE_IS_NOT_VALID);
        }
    }    
}
