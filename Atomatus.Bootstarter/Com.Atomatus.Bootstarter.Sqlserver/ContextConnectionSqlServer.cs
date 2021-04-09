using System.Text;

namespace Com.Atomatus.Bootstarter.Context.Configuration.Connection
{
    internal sealed class ContextConnectionSqlServer : ContextConnection
    {
        private const int DEFAULT_PORT = 1433;
        private const int DEFAULT_CONNECTION_TIMEOUT_IN_SEC = 30;

        public ContextConnectionSqlServer(Builder builder) : base(builder) { }

        protected override string GetConnectionString()
        {
            return new StringBuilder()
                .Append("Data Source=").AppendOrElse(host, ".")
                .Append(",").AppendOrElse(port, DEFAULT_PORT).Append(";")
                .Append("Initial Catalog=").AppendOrThrow(database, "Database name not set!").Append(";")
                .AppendIf(HasUsername(), "User Id=", user, ';')
                .AppendIf(HasPassword(), "Password=", password, ';')
                .AppendIf(HasUsernameAndPassword() || !DotnetRunningInContainer, "Integrated Security=True;")
                .AppendIf(HasUsernameAndPassword(), "Trusted_Connection=True;")
                .Append("MultipleActiveResultSets=True;")
                .Append("Connection Timeout=").AppendOrElse(timeout, DEFAULT_CONNECTION_TIMEOUT_IN_SEC).Append(";")
                .AppendIf(HasIdleLifetime(), "Connection Lifetime=", idleLifetime, ';')                
                .AppendIf(MinPoolSize(), "Min Pool Size=", minPoolSize, ';')
                .AppendIf(MaxPoolSize(), "Max Pool Size=", maxPoolSize, ";Pooling=true;")
                .ToString();
        }
    }
}
