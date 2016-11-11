using System;
using System.Threading;
using System.Threading.Tasks;
using Csys.Common;
using HJPT.Models;

namespace Csys.Identity
{
    public interface IUserStore : IDisposable
    {
        Task<TaskResult> CreateAsync(User user, CancellationToken cancellationToken);
        Task<TaskResult> UpdateAsync(User user, CancellationToken cancellationToken);
        Task<User> FindByIdAsync(string id, CancellationToken cancellationToken);
        Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken);
        Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken);
        Task<User> FindByPassKeyAsync(string passKey, CancellationToken cancellationToken);
    }
}
