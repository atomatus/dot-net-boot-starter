using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [Collection("ExplicitContextService")]
    public sealed class UnitTestBaseForClientImplExplicitContextServiceAsync : UnitTestBaseForClient<ProviderFixtureImplExplicitContext<ClientContext, ClientService, ClientTest, long>>
    {
        public UnitTestBaseForClientImplExplicitContextServiceAsync(ProviderFixtureImplExplicitContext<ClientContext, ClientService, ClientTest, long> provider) : base(provider)
        {

        }
    }
}
