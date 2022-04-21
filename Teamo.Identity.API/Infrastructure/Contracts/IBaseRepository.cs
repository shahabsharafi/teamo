using MongoDB.Driver;

namespace Teamo.Identity.API.Infrastructure.Contracts
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        public IMongoCollection<TEntity> DbSet { get; }
    }
}
