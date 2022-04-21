using MongoDB.Driver;
using MongoDbGenericRepository;
using Teamo.Infrastructure.Contracts;

namespace Teamo.Identity.API.Infrastructure.Repository
{
    public abstract class BaseRepository<TEntity> : AppMongoRepository 
    {
        protected readonly IAppMongoDbContext _context;
        protected IMongoCollection<TEntity> _dbSet;

        protected BaseRepository(IAppMongoDbContext context) : base(context)
        {
            _context = context;

            _dbSet = _context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public IMongoCollection<TEntity> DbSet { get { return _dbSet; } }
    }
}
