using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [Collection("DynamicContext")]
    public sealed class UnitTestBaseForClientImplDynamicContextAsync : UnitTestBaseForClientAsync<ProviderFixtureImplDynamicContext<ClientTest, long>>
    {
        public UnitTestBaseForClientImplDynamicContextAsync(ProviderFixtureImplDynamicContext<ClientTest, long> provider) : base(provider)
        {

        }
    }
}
