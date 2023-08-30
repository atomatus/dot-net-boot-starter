using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter
{
    internal interface ICopyStrategy
    {
        bool TryHandle([NotNull] object source, [NotNull] object target);
    }
}
