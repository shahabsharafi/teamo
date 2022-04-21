using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDbGenericRepository;

namespace Teamo.Identity.API.Infrastructure.Repository
{
    public class AppMongoRepository : MongoRepository
    {
        private AppMongoRepository(string connectionString, string databaseName) 
            : base(connectionString, databaseName) 
        { 
        }

        public AppMongoRepository(IMongoDbContext mongoDbContext) : base(mongoDbContext)
        {
        }
    }
}
