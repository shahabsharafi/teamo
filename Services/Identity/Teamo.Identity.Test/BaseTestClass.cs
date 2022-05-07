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
        protected readonly string PASSWORD = "111111";
        protected readonly string WRONG_PASSWORD = "111112";
        protected readonly string NEW_PASSWORD = "111113";
        protected readonly string DELETED_USER_NAME = "deleted_user";
        protected readonly string TWO_FACTOR_ENABLED_USER_NAME = "two_factor_enabled_user";
        protected readonly ApplicationUser EXISTS_USER;
        protected readonly ApplicationUser DELETED_USER;
        protected readonly ApplicationUser TWO_FACTOR_ENABLED_USER;
        protected readonly string VERIFICATION_CODE = "222222";
        protected readonly string WRONG_VERIFICATION_CODE = "222221";
        protected readonly string RESET_TOKEN = "333333";
        protected readonly string WRONG_RESET_TOKEN = "333331";
        public BaseTestClass()
        {
            EXISTS_USER = new()
            {
                Email = EXISTS_EMAIL,
                UserName = EXISTS_USER_NAME,
                IsDeleted = false
            };

            DELETED_USER = new()
            {
                Email = EXISTS_EMAIL,
                UserName = EXISTS_USER_NAME,
                IsDeleted = true
            };

            TWO_FACTOR_ENABLED_USER = new()
            {
                Email = EXISTS_EMAIL,
                UserName = TWO_FACTOR_ENABLED_USER_NAME,
                IsDeleted = false,   
                TwoFactorEnabled = true
            };

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            
            _userManager = new Mock<FakeUserManager>();

            _signInManager = new Mock<FakeSignInManager>();

            _userManager.Setup(_ => _.FindByNameAsync(EXISTS_USER_NAME))
                .ReturnsAsync(() => EXISTS_USER);

            _userManager.Setup(_ => _.FindByEmailAsync(EXISTS_EMAIL))
                .ReturnsAsync(() => EXISTS_USER);

            _userManager.Setup(_ => _.FindByEmailAsync(DELETED_USER_NAME))
               .ReturnsAsync(() => DELETED_USER);

            _userManager.Setup(_ => _.FindByNameAsync(TWO_FACTOR_ENABLED_USER_NAME))
               .ReturnsAsync(() => TWO_FACTOR_ENABLED_USER);
        }
    }
}
