using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [Collection("DynamicContext")]
    public sealed class UnitTestBaseForClientImplDynamicContext : UnitTestBaseForClient<ProviderFixtureImplDynamicContext<ClientTest, long>>
    {
        public UnitTestBaseForClientImplDynamicContext(ProviderFixtureImplDynamicContext<ClientTest, long> provider) : base(provider)
        {

        }
    }
}
