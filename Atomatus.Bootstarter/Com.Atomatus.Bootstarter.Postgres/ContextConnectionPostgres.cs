using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context connection for postgres database.
    /// </summary>
    internal sealed class ContextConnectionPostgres : ContextConnection
    {
        internal const int VERSION_MAJOR = 9;
        internal const int VERSION_MINOR = 6;

        private const int DEFAULT_PORT = 5432;
        private const int DEFAULT_CONNECTION_TIMEOUT_IN_SEC = 120;

        public ContextConnectionPostgres(Builder builder) : base(builder) { }

        protected override string GetConnectionString()
        {
            return new StringBuilder()
                .Append("Server=").AppendOrElse(host, "localhost").Append(';')
                .Append("Port=").AppendOrElse(port, DEFAULT_PORT).Append(';')
                .AppendIf(IsRequireDatabase(), s0 => s0.Append("Database=").AppendOrThrow(database, "database name not set!").Append(';'))
                .AppendIf(HasUsername(), "User Id=", user, ';')
                .AppendIf(HasPassword(), "Password=", password, ';')
                .AppendIf(HasNotUsernameAndPassword() || !DotnetRunningInContainer || IsIntegratedSecurity(), "Integrated Security=", !HasIntegratedSecurity() || IsIntegratedSecurity(), ';')
                .Append("Timeout=").AppendOrElse(timeout, DEFAULT_CONNECTION_TIMEOUT_IN_SEC).Append(';')
                .Append("Command Timeout=").AppendOrElse(commandTimeout, DEFAULT_CONNECTION_TIMEOUT_IN_SEC).Append(';')
                .AppendIf(MinPoolSize(), "Min Pool Size=", minPoolSize, ';')
                .AppendIf(MaxPoolSize(), "Max Pool Size=", maxPoolSize, ";Pooling=true;")
                .AppendIf(HasIdleLifetime(), "Connection Idle Lifetime=", idleLifetime, ';')
                .AppendIf(HasEncrypt(), "SslMode=", IsEncrypt() ? (HasTrustServerCertificate() ? (IsTrustServerCertificate() ? "TrustCertificate" : "Prefer") : "Require") : "Disable", ';')
                .AppendIf(HasTrustServerCertificate(), "Trust Server Certificate=", IsTrustServerCertificate(), ';')
                .Append("ApplicationName=").AppendOrElse(applicationName, Assembly.GetEntryAssembly().GetName().Name).Append(';')
                .ToString();
        }

        protected internal override DbContextOptionsBuilder Attach(DbContextOptionsBuilder options)
        {
            return options
                .UseNpgsql(o => InvokeOptions(o.SetPostgresVersion(VERSION_MAJOR, VERSION_MINOR)))
                .UseNpgsql(this);
        }
    }
}
