using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Services;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [CollectionDefinition("ExplicitContextService")]
    public class ProviderFixtureImplExplicitContext<TContext, TService, TEntity, TID> : ProviderFixture<TEntity, TID>
        where TContext : ContextBase
        where TService : ServiceCrud<TContext, TEntity, TID>
        where TEntity  : class, IModel<TID>, new()
    {
        protected override void OnConfigureServices(IServiceCollection services)
        {
            string dbName = Path.Join(Directory.GetCurrentDirectory(), "dbTestexcs.db");
            services
                .AddDbContextAsSqlite<TContext>(b => b.Database(dbName))
                .AddScoped<IServiceCrud<TEntity, TID>, TService>()
                .AddScoped<IServiceCrudAsync<TEntity, TID>, TService>()
                .AddScoped<IServiceCrud<TEntity>, TService>()
                .AddScoped<IServiceCrudAsync<TEntity>, TService>();
        }
    }
}
