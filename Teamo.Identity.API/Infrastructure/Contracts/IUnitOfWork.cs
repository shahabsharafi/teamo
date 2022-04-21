using Teamo.Identity.API.Infrastructure.Contracts;

namespace Teamo.Infrastructure.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> Commit();
        IUserRepository UserRepository { get; } 
    }
}
