using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Util
{
    internal interface ICopyStrategy
    {
        bool TryHandle([NotNull] object source, [NotNull] object target);
    }
}
