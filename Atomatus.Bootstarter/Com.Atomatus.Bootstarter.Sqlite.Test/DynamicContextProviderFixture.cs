using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [CollectionDefinition("DynamicContextProvider")]
    public class DynamicContextProviderFixture : IAsyncLifetime
    {
        private ServiceProvider provider;
                
        public IServiceProvider Provider => provider ?? throw new ObjectDisposedException(nameof(DynamicContextProviderFixture));

        public IServiceCrud<ClientTest, long> ClientServiceWithId => Provider.GetServiceTo<ClientTest, long>();
        
        public IServiceCrud<ClientTest> ClientService => Provider.GetServiceTo<ClientTest>();

        public IServiceCrudAsync<ClientTest, long> ClientServiceWithIdAsync => Provider.GetServiceAsyncTo<ClientTest, long>();

        public IServiceCrudAsync<ClientTest> ClientServiceAsync => Provider.GetServiceAsyncTo<ClientTest>();

        private static int count;

        public ServiceProvider BuildServiceProvider()
        {
            string dbName = $"dbTest{Interlocked.Increment(ref count)}.db";
            IServiceCollection services = new ServiceCollection();
            services.AddDbContextAsSqlite(
                b => b.Database(dbName)
                      .EnsureCreated()
                      .EnsureDeletedOnDispose(),
                s => s.AddServiceTo<ClientTest>());

            return services.BuildServiceProvider();
        }

        public async Task InitializeAsync()
        {
            provider = await Task.Factory.StartNew(BuildServiceProvider);
        }

        public async Task DisposeAsync()
        {
            await provider?.DisposeAsync().AsTask();
            provider = null;
        }
    }
}
