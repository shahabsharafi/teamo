using AutoMapper;
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
    public class RegisterUserTest : BaseTestClass
    {

        private readonly Mock<ILogger<RegisterUserHandler>> _logger;
        private readonly Mock<IMapper> _mapper;
        private readonly RegisterUserHandler _handler;
        public RegisterUserTest()
        {
            _mapper = new Mock<IMapper>();
            _mapper.Setup(_ => _.Map<ApplicationUser>(It.IsAny<RegisterUserCommand>()))
                .Returns((RegisterUserCommand command) => new ApplicationUser() { 
                    Email = command.Email,
                    FullName = command.FullName,
                    UserName = command.UserName
                });
            _logger = new Mock<ILogger<RegisterUserHandler>>();

            _handler =
                new(_logger.Object, _mapper.Object, _userManager.Object);
        }

        [Fact]
        public async void Test_RegisterUser_If_User_Duplicated()
        {
            RegisterUserCommand command = new()
            {
                FullName = FIULL_NAME,
                Email = NEW_EMAIL,
                UserName = EXISTS_USER_NAME,
                Password = PASSWORD
            };

            _userManager.Setup(_ => _.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(() => IdentityResult.Success);

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command));

            Assert.True(exception.Message == Constants.Message.DUPLICATE_USER);
        }

        [Fact]
        public async void Test_RegisterUser_If_Email_Duplicated()
        {
            RegisterUserCommand command = new()
            {
                FullName = FIULL_NAME,
                Email = EXISTS_EMAIL,
                UserName = NEW_USER_NAME,
                Password = PASSWORD
            };

            _userManager.Setup(_ => _.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(() => IdentityResult.Success);

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command));

            Assert.True(exception.Message == Constants.Message.DUPLICATE_EMAIL);
        }

        [Fact]
        public async void Test_RegisterUser_If_Register_Failed()
        {
            RegisterUserCommand command = new()
            {
                FullName = FIULL_NAME,
                Email = NEW_EMAIL,
                UserName = NEW_USER_NAME,
                Password = PASSWORD
            };

            _userManager.Setup(_ => _.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(() => IdentityResult.Failed());

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command));

            Assert.True(exception.Message == Constants.Message.REGISTER_FALED);
        }

        [Fact]
        public async void Test_RegisterUser()
        {
            RegisterUserCommand command = new()
            {
                FullName = FIULL_NAME,
                Email = NEW_EMAIL,
                UserName = NEW_USER_NAME,
                Password = PASSWORD
            };

            _userManager.Setup(_ => _.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(() => IdentityResult.Success);

            Exception exception = await Record.ExceptionAsync(async () => await _handler.Handle(command));

            _userManager.Verify(_ => _.FindByNameAsync(It.IsAny<string>()));

            _userManager.Verify(_ => _.FindByEmailAsync(It.IsAny<string>()));

            _mapper.Verify(_ => _.Map<ApplicationUser>(It.IsAny<RegisterUserCommand>()));

            Assert.Null(exception);
        }
    }
}