using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context Connection for InMemory.
    /// </summary>
    internal sealed class ContextConnectionInMemory : ContextConnection
    {
        public ContextConnectionInMemory(Builder builder) : base(builder) { }

        protected override string GetConnectionString()
        {
            return database;
        }

        protected internal override DbContextOptionsBuilder Attach(DbContextOptionsBuilder options)
        {
            return options.UseInMemoryDatabase(this);
        }
    }
}