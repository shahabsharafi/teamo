using Teamo.Identity.API.Infrastructure.Contracts;
using Teamo.Identity.API.Infrastructure.Models;
using Teamo.Infrastructure.Contracts;

namespace Teamo.Identity.API.Infrastructure.Repository
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(IAppMongoDbContext context) : base(context)
        {
        }
    }
}
