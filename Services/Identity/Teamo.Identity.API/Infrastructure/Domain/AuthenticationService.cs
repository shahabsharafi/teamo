using Microsoft.AspNetCore.Identity;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Domain
{
    public interface IAuthenticationService
    {
        Task UnsuccessfulAuthentication(ApplicationUser user);
    }

    public class AuthenticationService : IAuthenticationService
    {
       private readonly UserManager<ApplicationUser> _userManager;
        public AuthenticationService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task UnsuccessfulAuthentication(ApplicationUser user)
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
    }
}
