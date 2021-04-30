using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    public abstract class ProviderFixture<TEntity, TID> : IAsyncLifetime
        where TEntity : IModel<TID>
    {
        #region Properties
        private ServiceProvider provider;
                
        public IServiceProvider Provider => provider ?? throw new ObjectDisposedException(this.GetType().Name);

        public IServiceCrud<TEntity, TID> ServiceWithId => Provider.GetServiceTo<TEntity, TID>();
        
        public IServiceCrud<TEntity> Service => Provider.GetServiceTo<TEntity>();

        public IServiceCrudAsync<TEntity, TID> ServiceWithIdAsync => Provider.GetServiceAsyncTo<TEntity, TID>();

        public IServiceCrudAsync<TEntity> ServiceAsync => Provider.GetServiceAsyncTo<TEntity>();
        #endregion

        #region OnConfigureServices
        protected abstract void OnConfigureServices(IServiceCollection services);

        private ServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            this.OnConfigureServices(services);
            return services.BuildServiceProvider();
        }
        #endregion

        #region IAsyncLifetime
        async Task IAsyncLifetime.InitializeAsync()
        {
            provider = await Task.Factory.StartNew(BuildServiceProvider);
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await provider?.DisposeAsync().AsTask();
            provider = null;
        }
        #endregion
    }
}
