using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [Collection("ExplicitContext")]
    public sealed class UnitTestBaseForClientImplExplicitContext : UnitTestBaseForClient<ProviderFixtureImplExplicitContext<ClientContext, ClientTest, long>>
    {
        public UnitTestBaseForClientImplExplicitContext(ProviderFixtureImplExplicitContext<ClientContext, ClientTest, long> provider) : base(provider)
        {

        }
    }
}
