using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context Connection for Sqlite.
    /// </summary>
    internal sealed class ContextConnectionSqlite : ContextConnection
    {
        public ContextConnectionSqlite(Builder builder) : base(builder) { }

        protected override string GetConnectionString()
        {
            return new StringBuilder()
                .Append("Data Source=").AppendOrElse(database, ":memory:").Append(';')                
                .AppendIf(string.IsNullOrEmpty(host), "New=True;")
                .AppendIf(HasPassword(), "Password=", password, ';')
                .ToString();
        }

        protected internal override DbContextOptionsBuilder Attach(DbContextOptionsBuilder options)
        {
            return options.UseSqlite(this);
        }
    }
}
