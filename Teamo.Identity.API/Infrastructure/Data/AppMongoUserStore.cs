using AspNetCore.Identity.MongoDbCore;
using Microsoft.AspNetCore.Identity;
using MongoDbGenericRepository;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Data
{
    public class AppMongoUserStore 
        : MongoUserStore<ApplicationUser, ApplicationRole, IMongoDbContext, Guid>
    {
        public AppMongoUserStore(IMongoDbContext context, IdentityErrorDescriber describer = null) 
            : base(context, describer)
        {
        }

        override 
    }
}
