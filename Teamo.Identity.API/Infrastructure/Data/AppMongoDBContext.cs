using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbGenericRepository;
using Teamo.Infrastructure.Contracts;

namespace Teamo.Identity.API.Infrastructure.Data
{
    public class AppMongoDBContext : MongoDbContext, IAppMongoDbContext, IDisposable
    {
        public IClientSessionHandle? Session { get; set; }
        private readonly List<Func<Task>> _commands;
        public AppMongoDBContext(IOptions<MongoSettings> mongoSettings) 
            : base(CreateParameters(mongoSettings))
        {
        }

        private static IMongoDatabase CreateParameters(
            IOptions<MongoSettings> mongoSettings)
        {
            MongoUrl url = MongoUrl.Create(mongoSettings.Value.Mongodb);
            MongoClient mongoClient = new MongoClient(url);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(url.DatabaseName);
            return mongoDatabase;
        }

        public async Task<int> SaveChanges()
        {
            using (Session = await Database.Client.StartSessionAsync())
            {
                Session.StartTransaction();

                var commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await Session.CommitTransactionAsync();
            }

            return _commands.Count;
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }

        public void Dispose()
        {
            Session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
