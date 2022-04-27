using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.Test
{

    public class FakeUserManager : UserManager<ApplicationUser>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
        {
        }
    }

    public class FakeSignInManager : SignInManager<ApplicationUser>
    {
        public FakeSignInManager()
            : base(new Mock<FakeUserManager>().Object,
                new HttpContextAccessor(),
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object, 
                new Mock<IUserConfirmation<ApplicationUser>>().Object)
        {
        }
    }

    public class BaseTestClass
    {
        protected readonly Mock<FakeUserManager> _userManager;
        protected readonly Mock<FakeSignInManager> _signInManager;
        protected readonly string FIULL_NAME = "Name Family";
        protected readonly string EXISTS_EMAIL = "a@a.com";
        protected readonly string EXISTS_USER_NAME = "user";
        protected readonly string NEW_EMAIL = "b@b.com";
        protected readonly string NEW_USER_NAME = "new_user";
        protected readonly string PASSWORD = "123456";
        public BaseTestClass()
        {
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            
            _userManager = new Mock<FakeUserManager>();

            _signInManager = new Mock<FakeSignInManager>();

            _userManager.Setup(_ => _.FindByNameAsync(EXISTS_USER_NAME))
                .ReturnsAsync(() => new ApplicationUser() { });

            _userManager.Setup(_ => _.FindByEmailAsync(EXISTS_EMAIL))
                .ReturnsAsync(new ApplicationUser { UserName = NEW_EMAIL });            
        }
    }
}
