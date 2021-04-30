using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [CollectionDefinition("ExplicitContext")]
    public class ProviderFixtureImplExplicitContext<TContext, TEntity, TID> : ProviderFixture<TEntity, TID>
        where TContext : ContextBase
        where TEntity : IModel<TID>
    {
        protected override void OnConfigureServices(IServiceCollection services)
        {
            string dbName = Path.Join(Directory.GetCurrentDirectory(), "dbTestexc.db");
            services.AddDbContextAsSqlite<TContext>(
                b => b.Database(dbName),
                s => s.AddServiceTo<TEntity>());
        }
    }
}
