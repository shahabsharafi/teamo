using MongoDB.Driver;

namespace Teamo.Assets.MongoDB.Contracts
{
    public interface IMongoContext : IDisposable
    {
        void AddCommand(Func<Task> func);
        Task<int> SaveChanges();
        IMongoCollection<T> GetCollection<T>(string name);
        IMongoDatabase Database { get; }
    }
}
