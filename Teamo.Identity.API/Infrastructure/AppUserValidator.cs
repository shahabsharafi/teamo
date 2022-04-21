using Microsoft.AspNetCore.Identity;
using MongoDbGenericRepository;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure
{
    public class AppUserValidator
    {
        IMongoDbContext _mongoDbContext;
        public AppUserValidator(IMongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            var matchingUser = _mongoDbContext.Employees.FirstOrDefault(candidateUser => candidateUser.EmailAddress.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

            if (matchingUser == null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Sorry, your email address isn't recognised as an employee by ValidateAsync" }));
            }
            else
            {
                return Task.FromResult(IdentityResult.Success);
            }
        }
    }
}
