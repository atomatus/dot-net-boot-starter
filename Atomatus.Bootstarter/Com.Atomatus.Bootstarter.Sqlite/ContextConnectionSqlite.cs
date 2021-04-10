using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Com.Atomatus.Bootstarter.Context.Configuration.Connection
{
    internal sealed class ContextConnectionSqlite : ContextConnection
    {
        public ContextConnectionSqlite(Builder builder) : base(builder) { }

        protected override string GetConnectionString()
        {
            return new StringBuilder()
                .Append("Data Source=").AppendOrElse(database, ":memory:").Append(';')
                .Append("Version=3;")
                .AppendIf(string.IsNullOrEmpty(host), "New=True;")
                .AppendIf(HasPassword(), "Password=", password, ';')
                .AppendIf(MaxPoolSize(), "Pooling=True;Max Pool Size=", maxPoolSize, ';')
                .ToString();
        }

        protected internal override DbContextOptionsBuilder Attach(DbContextOptionsBuilder options)
        {
            return options.UseSqlite(this);
        }
    }
}
