using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [CollectionDefinition("DynamicContext")]
    public class ProviderFixtureImplDynamicContext<TEntity, TID> : ProviderFixture<TEntity, TID>
        where TEntity : IModel<TID>
    {
        protected override void OnConfigureServices(IServiceCollection services)
        {
            string dbName = Path.Join(Directory.GetCurrentDirectory(), "dbTestdyc.db");
            services.AddDbContextAsSqlite(
                b => b.Database(dbName)
                      .EnsureCreated()
                      .EnsureDeletedOnDispose(),
                s => s.AddServiceTo<TEntity>());
        }
    }
}
