using Com.Atomatus.Bootstarter.Context;
using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    public sealed class ClientContext : ContextBase
    {
        public DbSet<ClientTest> Clients { get; internal set; }

        public ClientContext(DbContextOptions options) : base(options) 
        {
            this.Database.EnsureCreated();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            this.Database.EnsureDeleted();
        }
    }
}
