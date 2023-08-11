using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Util
{
    internal sealed class CommonInterfaceCopyStrategy : ICopyStrategy
    {
        public bool TryHandle([NotNull] object source, [NotNull] object target)
        {
            var sInterfaces = source.GetType().GetInterfaces();
            var tInterfaces = target.GetType().GetInterfaces();
            var cInterfaces = sInterfaces.Where(tInterfaces.Contains).ToList();
            var handled = false;
            foreach (var cInterface in cInterfaces) 
            {
                var props = cInterface.GetProperties().Where(i => i.CanWrite && i.CanWrite);
                handled |= CommonPropertyCopyStrategy.TryHandleProperties(source, target, props, props);
            }

            return handled;
        }
    }
}
