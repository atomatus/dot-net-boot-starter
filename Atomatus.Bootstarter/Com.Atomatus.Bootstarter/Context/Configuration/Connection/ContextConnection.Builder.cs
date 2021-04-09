
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlserver")]
namespace Com.Atomatus.Bootstarter.Context.Configuration.Connection
{
    public abstract partial class ContextConnection : ContextConnectionParameters
    {
        public sealed class Builder : ContextConnectionParameters
        {
            private TryBuildContextConnectionCallback callback;

            #region Local Parameters
            internal delegate bool TryBuildContextConnectionCallback(Builder builder, out ContextConnection conn);

            internal Builder DatabaseType(DatabaseTypes databaseType)
            {
                this.databaseType = databaseType;
                return this;
            }

            internal Builder AddBuildCallback(TryBuildContextConnectionCallback callback)
            {
                this.callback = callback;
                return this;
            }
            #endregion

            #region Parameters
            public Builder DatabaseName(string database)
            {
                this.database = database;
                return this;
            }

            public Builder Host(string host)
            {
                this.host = host;
                return this;
            }

            public Builder Port(int port)
            {
                this.port = port;
                return this;
            }

            public Builder User(string user)
            {
                this.user = user;
                return this;
            }

            public Builder Password(string password)
            {
                this.password = password;
                return this;
            }

            public Builder Timeout(int timeoutInSec)
            {
                this.timeout = timeoutInSec;
                return this;
            }

            public Builder CommandTimeout(int commandTimeoutInSec)
            {
                this.commandTimeout = commandTimeoutInSec;
                return this;
            }

            public Builder IdleLifeTime(int idleLifeTimeInSec)
            {
                this.idleLifetime = idleLifeTimeInSec;
                return this;
            }

            public Builder MinPoolSize(int minPoolSize)
            {
                this.minPoolSize = minPoolSize;
                return this;
            }

            public Builder MaxPoolSize(int maxPoolSize)
            {
                this.maxPoolSize = maxPoolSize;
                return this;
            }

            public Builder ApplicationName(string applicationName)
            {
                this.applicationName = applicationName;
                return this;
            }
            #endregion

            #region Parameters From Environment
            public Builder DatabaseEnv(EnvironmentVariable database)
            {
                this.database = database;
                return this;
            }

            public Builder HostEnv(EnvironmentVariable host)
            {
                this.host = host;
                return this;
            }

            public Builder PortEnv(EnvironmentVariable port)
            {
                this.port = port;
                return this;
            }

            public Builder UserEnv(EnvironmentVariable user)
            {
                this.user = user;
                return this;
            }

            public Builder TimeoutEnv(EnvironmentVariable timeoutInSec)
            {
                this.timeout = timeoutInSec;
                return this;
            }

            public Builder CommandTimeoutEnv(EnvironmentVariable commandTimeoutInSec)
            {
                this.commandTimeout = commandTimeoutInSec;
                return this;
            }

            public Builder IdleLifeTimeEnv(EnvironmentVariable idleLifeTimeInSec)
            {
                this.idleLifetime = idleLifeTimeInSec;
                return this;
            }

            public Builder MinPoolSizeEnv(EnvironmentVariable minPoolSize)
            {
                this.minPoolSize = minPoolSize;
                return this;
            }

            public Builder MaxPoolSizeEnv(EnvironmentVariable maxPoolSize)
            {
                this.maxPoolSize = maxPoolSize;
                return this;
            }

            public Builder ApplicationNameEnv(EnvironmentVariable applicationName)
            {
                this.applicationName = applicationName;
                return this;
            }
            #endregion

            #region IDisposable
            protected override void OnDispose()
            {
                this.callback = null;
            }
            #endregion

            #region Build
            public ContextConnection Build()
            {
                using (this)
                {

                    if (callback != null && callback.Invoke(this, out ContextConnection conn))
                    {
                        return conn;
                    }
                    else if (string.IsNullOrWhiteSpace(connectionStringKey))
                    {

                    }

                    return null;
                }
            }
            #endregion
        }
    }
}
