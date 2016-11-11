using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Csys.Common;
using Csys.Data;
using Csys.Models;
using Microsoft.EntityFrameworkCore;

namespace Csys.Identity
{
    public class UserStore : IUserStore
    {
        public ApplicationDbContext Context { get; private set; }
        private bool _disposed;
        public UserStore(ApplicationDbContext context)
        {
            Context = context;
        }
        public virtual IQueryable<User> Users => Context.Set<User>();

        public async Task<TaskResult> CreateAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            Context.Add(user);
            await SaveChanges(cancellationToken);
            return TaskResult.Success;
        }

        public virtual Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<User> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        public Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<TaskResult> UpdateAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Context.Attach(user);
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            Context.Update(user);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return TaskResult.Failed(ErrorDescriber.ConcurrencyFailure);
            }
            return TaskResult.Success;;
        }

        private Task SaveChanges(CancellationToken cancellationToken)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
        public void Dispose()
        {
            _disposed = true;
        }
    }
}
