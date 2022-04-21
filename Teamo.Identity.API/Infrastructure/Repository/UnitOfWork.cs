using Teamo.Identity.API.Infrastructure.Contracts;
using Teamo.Infrastructure.Contracts;

namespace Teamo.Identity.API.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IAppMongoDbContext _context;
        IUserRepository _userRepository;

        public UnitOfWork(IAppMongoDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public IUserRepository UserRepository => _userRepository;

        public async Task<bool> Commit()
        {
            var changeAmount = await _context.SaveChanges();

            return changeAmount > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
