using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading;

namespace Com.Atomatus.Bootstarter.Context.Ensures
{
    /// <summary>
    /// Context base for ensure creation definition.
    /// </summary>
    public abstract class ContextBaseForEnsure : ContextBase
    {
        private class AtomicCounter 
        {
            private long value;
            
            public long Value { get => Interlocked.Read(ref value); }

            public long Increment() => Interlocked.Increment(ref value);

            public long Decrement() => Interlocked.Decrement(ref value);
        }

        private static readonly ConcurrentDictionary<string, AtomicCounter> counters;

        private readonly string key;

        static ContextBaseForEnsure()
        {
            counters = new ConcurrentDictionary<string, AtomicCounter>();
        }

        /// <summary>
        /// Context base constructor receiving build parameters options,
        /// database schema name and defining whether attempt to load entity's
        /// configuration for each dbSet declared in context.
        /// </summary>
        /// <param name="options">
        /// The options for this context.
        /// </param>
        protected ContextBaseForEnsure(DbContextOptions options) : base(options)  
        {
            this.key = options.ContextType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Check if current context can request any ensure creation,
        /// otherwise indicates that already did it before.
        /// </summary>
        /// <returns></returns>
        protected bool CanEnsure()
        {
            return counters.GetOrAdd(key, k => new AtomicCounter()).Increment() == 1L;
        }

        /// <summary>
        /// Check if current context can remove ensure creations.
        /// </summary>
        /// <returns></returns>
        protected bool IsEnsured()
        {
            return !counters.TryGetValue(key, out AtomicCounter counter) ||
                (counter.Decrement() <= 0L && counters.TryRemove(key, out _));
        }
    }
}
