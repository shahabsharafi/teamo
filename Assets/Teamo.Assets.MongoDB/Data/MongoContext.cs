
using Microsoft.Extensions.Options;
using Teamo.Assets.MongoDB.Contracts;
using MongoDB.Driver;

namespace Teamo.Assets.MongoDB.Data
{
    public class MongoContext : IMongoContext
    {
        private IClientSessionHandle? _session;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly List<Func<Task>> _commands;

        public MongoContext(IOptions<ConnectionSettings> options)
        {
            MongoUrl url = MongoUrl.Create(options.Value.MongoConnection);
            _mongoClient = new MongoClient(url);
            _database = _mongoClient.GetDatabase(url.DatabaseName);

            // Every command will be stored and it'll be processed at SaveChanges
            _commands = new List<Func<Task>>();
        }

        public async Task<int> SaveChanges()
        {
            using (_session = await _mongoClient.StartSessionAsync())
            {
                _session.StartTransaction();

                var commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await _session.CommitTransactionAsync();
            }

            return _commands.Count;
        }

        public IMongoDatabase Database => _database;

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            _session?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }
    }
}
