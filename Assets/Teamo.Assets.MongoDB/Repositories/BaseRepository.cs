using Teamo.Assets.MongoDB.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Teamo.Assets.MongoDB.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly IMongoContext _context;
        protected IMongoCollection<TEntity> _dbSet;

        protected BaseRepository(IMongoContext context)
        {
            _context = context;

            _dbSet = _context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public virtual void Add(TEntity obj)
        {
            _context.AddCommand(() => _dbSet.InsertOneAsync(obj));
        }

        public virtual async Task<TEntity> GetById(string id)
        {
            var data = await _dbSet.FindAsync(Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id)));
            return data.SingleOrDefault();
        }

        public virtual async Task<IQueryable<TEntity>> GetAll()
        {
            return await Task.FromResult(_dbSet.AsQueryable());
        }

        public virtual void Update(TEntity obj)
        {
            string id = obj.GetId();
            _context.AddCommand(() => _dbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id)), obj));
        }

        public virtual void Remove(string id)
        {
            _context.AddCommand(() => _dbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id))));
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
