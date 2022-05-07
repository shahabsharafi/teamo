using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Teamo.Identity.API.Infrastructure;
using Teamo.Identity.API.Infrastructure.Domain;
using Teamo.Identity.API.Infrastructure.Models;
using Xunit;

namespace Teamo.Identity.Test
{
    public class ChangePasswordTest : BaseTestClass
    {
        private readonly Mock<ILogger<ChangePasswordHandler>> _logger;
        private readonly ChangePasswordHandler _handler;
        private readonly string NEW_PASSWORD = "654321";
        public ChangePasswordTest() : base()
        {            
            _signInManager.Setup(_ => _.RefreshSignInAsync(It.IsAny<ApplicationUser>()));

            _logger = new Mock<ILogger<ChangePasswordHandler>>();

            _handler =
                new(_logger.Object, _userManager.Object, _signInManager.Object);
        }

        [Fact]
        public async void Test_ChangePassword()
        {
            ChangePasswordCommand command = new()
            {
                UserName = EXISTS_USER_NAME,
                OldPassword = PASSWORD,
                NewPassword = NEW_PASSWORD
            };

            _userManager.Setup(_ => _.ChangePasswordAsync(It.IsAny<ApplicationUser>(), PASSWORD, It.IsAny<string>()))
                .ReturnsAsync(() => IdentityResult.Success);

            _userManager.Setup(_ => _.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(() => IdentityResult.Success);            

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command));

            _userManager.Verify(_ => _.ChangePasswordAsync(It.IsAny<ApplicationUser>(), PASSWORD, It.IsAny<string>()));

            _userManager.Verify(_ => _.UpdateAsync(It.IsAny<ApplicationUser>()));

            _signInManager.Verify(_ => _.RefreshSignInAsync(It.IsAny<ApplicationUser>()));

            Assert.Null(exception);
        }

        [Fact]
        public async void Test_RegisterUser_If_User_Is_Not_Found()
        {
            ChangePasswordCommand command = new()
            {
                UserName = NEW_USER_NAME,
                OldPassword = PASSWORD,
                NewPassword = NEW_PASSWORD
            };

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command));

            Assert.Equal(exception.Message, Constants.Message.USER_NOT_FOUND);
        }

        [Fact]
        public async void Test_RegisterUser_If_Change_Password_Is_Failed()
        {
            ChangePasswordCommand command = new()
            {
                UserName = EXISTS_USER_NAME,
                OldPassword = PASSWORD,
                NewPassword = NEW_PASSWORD
            };

            _userManager.Setup(_ => _.ChangePasswordAsync(It.IsAny<ApplicationUser>(), PASSWORD, It.IsAny<string>()))
                .ReturnsAsync(() => IdentityResult.Failed());

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command));

            _userManager.Verify(_ => _.ChangePasswordAsync(It.IsAny<ApplicationUser>(), PASSWORD, It.IsAny<string>()));

            _userManager.Verify(_ => _.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);

            Assert.Equal(exception.Message, Constants.Message.CHANGE_PASSWORD_FAILD);
        }

        [Fact]
        public async void Test_RegisterUser_If_Update_Is_Failed()
        {
            ChangePasswordCommand command = new()
            {
                UserName = EXISTS_USER_NAME,
                OldPassword = PASSWORD,
                NewPassword = NEW_PASSWORD
            };

            _userManager.Setup(_ => _.ChangePasswordAsync(It.IsAny<ApplicationUser>(), PASSWORD, It.IsAny<string>()))
                .ReturnsAsync(() => IdentityResult.Success);

            _userManager.Setup(_ => _.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(() => IdentityResult.Failed());

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command));

            _userManager.Verify(_ => _.ChangePasswordAsync(It.IsAny<ApplicationUser>(), PASSWORD, It.IsAny<string>()));

            _userManager.Verify(_ => _.UpdateAsync(It.IsAny<ApplicationUser>()));

            Assert.Equal(exception.Message, Constants.Message.CHANGE_PASSWORD_FAILD);
        }
    }    
}
