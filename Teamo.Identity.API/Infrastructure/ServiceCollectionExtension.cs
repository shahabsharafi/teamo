using AspNetCore.Identity.MongoDbCore;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDbGenericRepository;
using Teamo.Identity.API.Infrastructure.Data;
using Teamo.Identity.API.Infrastructure.Models;
using Teamo.Identity.API.Infrastructure.Repository;

namespace Teamo.Identity.API.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IdentityBuilder AddMongoDBIdentity(this IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentity<ApplicationUser, ApplicationRole>();
            builder.Services.TryAddSingleton<IMongoDbContext, AppMongoDBContext>();
            builder.Services.TryAddSingleton<IMongoRepository, AppMongoRepository>();
            builder.Services.TryAddScoped<IUserStore<ApplicationUser>>(provider =>
            {
                return new MongoUserStore<ApplicationUser, ApplicationRole, IMongoDbContext, Guid>(provider.GetService<IMongoDbContext>());
            });

            builder.Services.TryAddScoped<IRoleStore<ApplicationRole>>(provider =>
            {
                return new MongoRoleStore<ApplicationRole, IMongoDbContext, Guid>(provider.GetService<IMongoDbContext>());
            });
            builder.AddUserValidator<ApplicationUser>();
            builder.AddDefaultTokenProviders();
            return builder;
        }
    }
}
