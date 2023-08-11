using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Util
{
    internal class CopyHandler : IDisposable
    {
        private Func<ICopyStrategy> constructor;
        private CopyHandler next;

        internal CopyHandler([NotNull] Func<ICopyStrategy> constructor)
        {
            this.constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        public CopyHandler Next([NotNull] Func<ICopyStrategy> nextStrategy)
        {
            return this.next = new CopyHandler(nextStrategy);
        }

        public bool Handle([NotNull] object source, [NotNull] object target)
        {
            return this.OnHandle(
                source ?? throw new ArgumentNullException(nameof(source)),
                target ?? throw new ArgumentNullException(nameof(target)));
        }

        private bool OnHandle([NotNull] object source, [NotNull] object target)
        {
            return (constructor != null && constructor().TryHandle(source, target)) ||
                (this.next != null && this.next.OnHandle(source, target));
        }

        public void Dispose()
        {
            this.next?.Dispose();
            this.next = null;
            this.constructor = null;
        }
    }
}
