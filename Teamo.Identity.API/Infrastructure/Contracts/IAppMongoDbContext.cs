using MongoDB.Driver;
using MongoDbGenericRepository;

namespace Teamo.Infrastructure.Contracts
{
    public interface IAppMongoDbContext : IMongoDbContext, IDisposable
    {
        void AddCommand(Func<Task> func);
        Task<int> SaveChanges();
    }
}
