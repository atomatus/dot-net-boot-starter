using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [Collection("ExplicitContextService")]
    public sealed class UnitTestBaseForClientImplExplicitContextService : UnitTestBaseForClient<ProviderFixtureImplExplicitContext<ClientContext, ClientService, ClientTest, long>>
    {
        public UnitTestBaseForClientImplExplicitContextService(ProviderFixtureImplExplicitContext<ClientContext, ClientService, ClientTest, long> provider) : base(provider)
        {

        }
    }
}
