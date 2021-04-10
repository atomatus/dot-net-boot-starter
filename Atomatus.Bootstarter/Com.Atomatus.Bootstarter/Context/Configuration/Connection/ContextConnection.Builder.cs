
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Cosmos")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Postgres")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlite")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlserver")]
namespace Com.Atomatus.Bootstarter.Context.Configuration.Connection
{
    public abstract partial class ContextConnection : ContextConnectionParameters
    {
        public sealed class Builder : ContextConnectionParameters
        {
            #region Local Parameters
            internal delegate bool TryBuildContextConnectionCallback(Builder builder, out ContextConnection conn);

            private List<TryBuildContextConnectionCallback> callbacks;
            
            internal Builder AddBuildCallback(TryBuildContextConnectionCallback callback)
            {
                this.callbacks ??= new List<TryBuildContextConnectionCallback>();                
                this.callbacks.Remove(callback);
                this.callbacks.Add(callback);
                return this;
            }

            internal Builder AddConfiguration(IConfiguration configuration)
            {
                this.configuration = configuration;
                return this;
            }

            internal Builder AddConnectionStringKey(string connectionStringKey)
            {
                this.connectionStringKey = connectionStringKey;
                return this;
            }
            #endregion

            #region Parameters
            internal Builder Database<TContext>() where TContext : ContextBase
            {
                string Replace(string target, string value) =>
                    target.Replace(value, string.Empty,
                    StringComparison.CurrentCultureIgnoreCase);                
                string name = Replace(Replace(typeof(TContext).Name, "Context"), "db").ToLower() + "db";
                return Database(name);
            }

            public Builder Database(string database)
            {
                this.database = database;
                return this;
            }

            public Builder Host(string host)
            {
                int index;
                if(host != null && (index = host.IndexOf(':')) != -1)
                {
                    this.host = host.Substring(0, index);
                    this.port = int.TryParse(
                        Regex.Replace(host[(index + 1)..], "[^0-9]", ""), out port) ? port : this.port;
                }
                else
                {
                    this.host = host;
                }

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

            #region Parameter Endpoint
            public Builder AccountEndpoint(string accountEndpoint)
            {
                if(string.IsNullOrWhiteSpace(accountEndpoint))
                {
                    host = null;
                    port = 0;
                    return this;
                } 
                else
                {
                    return Host(accountEndpoint);
                }
            }

            public Builder AccountKey(string accountKey)
            {
                this.password = accountKey;
                return this;
            }
            #endregion

            #region IDisposable
            protected override void OnDispose()
            {
                this.callbacks?.Clear();
                this.callbacks = null;
            }
            #endregion

            #region Build
            public ContextConnection Build()
            {
                using (this)
                {
                    if(this.callbacks != null)
                    {
                        var aux = this.callbacks.AsReadOnly();

                        foreach (var callback in aux)
                        {
                            if (callback.Invoke(this, out ContextConnection conn))
                            {
                                return conn;
                            }
                        }
                    }

                    throw new InvalidOperationException("Is not possible build a contextConnection without minimum data:" +
                           "You must set an IConfiguration and/or an database type!");
                }
            }
            #endregion
        }
    }
}
