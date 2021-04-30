using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [Collection("ExplicitContext")]
    public sealed class UnitTestBaseForClientImplExplicitContextAsync : UnitTestBaseForClientAsync<ProviderFixtureImplExplicitContext<ClientContext, ClientTest, long>>
    {
        public UnitTestBaseForClientImplExplicitContextAsync(ProviderFixtureImplExplicitContext<ClientContext, ClientTest, long> provider) : base(provider)
        {

        }
    }
}
