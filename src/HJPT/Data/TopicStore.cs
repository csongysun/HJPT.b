

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HJPT.Models;

namespace HJPT.Data
{
    public interface ITopicStore : IDisposable
    {
        IQueryable<Topic> Topics {get;}
    }

    public class TopicStore : ITopicStore
    {

        public ApplicationDbContext Context { get; private set; }
        private bool _disposed;
        public TopicStore(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }
        
        public IQueryable<Topic> Topics => Context.Set<Topic>();

        #region store feature
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
        #endregion



    }
}
