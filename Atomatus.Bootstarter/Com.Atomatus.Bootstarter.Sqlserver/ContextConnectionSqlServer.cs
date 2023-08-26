using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context connection fro SqlServer.
    /// </summary>
    internal sealed class ContextConnectionSqlServer : ContextConnection
    {
        private const int DEFAULT_PORT = 1433;
        private const int DEFAULT_CONNECTION_TIMEOUT_IN_SEC = 30;

        public ContextConnectionSqlServer(Builder builder) : base(builder) { }

        protected override string GetConnectionString()
        {
            return new StringBuilder()
                .Append("Data Source=").AppendOrElse(host, ".")
                .Append(',').AppendOrElse(port, DEFAULT_PORT).Append(';')
                .Append("Initial Catalog=").AppendOrThrow(database, "Database name not set!").Append(';')
                .AppendIf(HasUsername(), "User Id=", user, ';')
                .AppendIf(HasPassword(), "Password=", password, ';')
                .AppendIf(HasNotUsernameAndPassword() || !DotnetRunningInContainer || IsIntegratedSecurity(), "Integrated Security=True;")
                .AppendIf(HasNotUsernameAndPassword() || IsIntegratedSecurity(), "Trusted_Connection=True;")
                .AppendIf(IsReadOnly(), "ApplicationIntent=ReadOnly;")
                .AppendIf(IsMultipleActiveResultSets(), "MultipleActiveResultSets=True;")
                .AppendIf(IsEncrypt(), "Encrypt=True;")
                .AppendIf(IsTrustServerCertificate(), "TrustServerCertificate=True;")
                .Append("Connection Timeout=").AppendOrElse(timeout, DEFAULT_CONNECTION_TIMEOUT_IN_SEC).Append(';')
                .AppendIf(HasIdleLifetime(), "Connection Lifetime=", idleLifetime, ';')                
                .AppendIf(MinPoolSize(), "Min Pool Size=", minPoolSize, ';')
                .AppendIf(MaxPoolSize(), "Max Pool Size=", maxPoolSize, ";Pooling=true;")
                .Append("ApplicationName=").AppendOrElse(applicationName, Assembly.GetEntryAssembly().GetName().Name).Append(';')
                .ToString();
        }

        protected internal override DbContextOptionsBuilder Attach(DbContextOptionsBuilder options)
        {
            return options.UseSqlServer(this);
        }
    }
}
