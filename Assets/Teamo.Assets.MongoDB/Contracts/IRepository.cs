namespace Teamo.Assets.MongoDB.Contracts
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        void Add(TEntity obj);
        Task<TEntity> GetById(string id);
        Task<IQueryable<TEntity>> GetAll();
        void Update(TEntity obj);
        void Remove(string id);
    }
}
