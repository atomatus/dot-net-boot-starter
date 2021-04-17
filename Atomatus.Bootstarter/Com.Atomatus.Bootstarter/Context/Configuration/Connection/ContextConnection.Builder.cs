﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Cosmos")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Postgres")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlite")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlserver")]
namespace Com.Atomatus.Bootstarter.Context
{
    public abstract partial class ContextConnection : ContextConnectionParameters
    {
        /// <summary>
        /// Context connection builder.
        /// </summary>
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

            internal Builder AddDefaultConnectionStringOperation(ConnectionStringFunction defaultConnectionStringCallback)
            {
                this.connectionStringCallback = defaultConnectionStringCallback;
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

            /// <summary>
            /// Target database name.
            /// </summary>
            /// <param name="database">database name</param>
            /// <returns>current builder</returns>
            public Builder Database(string database)
            {
                this.database = database;
                return this;
            }

            /// <summary>
            /// <para>
            /// Target database host address with or without port.
            /// </para>
            /// <i>
            ///  Examples:<br/>
            ///  ● localhost<br/> 
            ///  ● localhost:1433<br/> 
            ///  ● mssqlserver<br/> 
            ///  ● mssqlserver:1433<br/> 
            ///  ● https://dbaddress.azure.com:443
            /// </i>
            /// </summary>
            /// <param name="host">host dns or ip address with or without port</param>
            /// <returns>current builder</returns>
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

            /// <summary>
            /// Target port
            /// </summary>
            /// <param name="port"></param>
            /// <returns>current builder</returns>
            public Builder Port(int port)
            {
                this.port = port;
                return this;
            }

            /// <summary>
            /// Database host authentication username.
            /// </summary>
            /// <param name="user">authentication username</param>
            /// <returns>current builder</returns>
            public Builder User(string user)
            {
                this.user = user;
                return this;
            }

            /// <summary>
            /// Database host authentication password.
            /// </summary>
            /// <param name="password">authentication password</param>
            /// <returns>current builder</returns>
            public Builder Password(string password)
            {
                this.password = password;
                return this;
            }

            /// <summary>
            /// Attempt database connection timeout in seconds.
            /// </summary>
            /// <param name="timeoutInSec">database connection timeout in seconds</param>
            /// <returns>current builder</returns>
            public Builder Timeout(int timeoutInSec)
            {
                this.timeout = timeoutInSec;
                return this;
            }

            /// <summary>
            /// Database command timeout in seconds.
            /// </summary>
            /// <param name="commandTimeoutInSec">database command  timeout in seconds</param>
            /// <returns>current builder</returns>
            public Builder CommandTimeout(int commandTimeoutInSec)
            {
                this.commandTimeout = commandTimeoutInSec;
                return this;
            }

            /// <summary>
            /// Idle Database lifetime timeout in seconds.
            /// </summary>
            /// <param name="idleLifeTimeInSec">idle lifetime timeout in seconds</param>
            /// <returns>current builder</returns>
            public Builder IdleLifeTime(int idleLifeTimeInSec)
            {
                this.idleLifetime = idleLifeTimeInSec;
                return this;
            }

            /// <summary>
            /// Min pool size.
            /// </summary>
            /// <param name="minPoolSize">Min pool size</param>
            /// <returns>current builder</returns>
            public Builder MinPoolSize(int minPoolSize)
            {
                this.minPoolSize = minPoolSize;
                return this;
            }

            /// <summary>
            /// Max pool size.
            /// </summary>
            /// <param name="maxPoolSize">Max pool size</param>
            /// <returns>current builder</returns>
            public Builder MaxPoolSize(int maxPoolSize)
            {
                this.maxPoolSize = maxPoolSize;
                return this;
            }

            /// <summary>
            /// Some database can identify database connection for application name.
            /// </summary>
            /// <param name="applicationName">current application name</param>
            /// <returns>current builder</returns>
            public Builder ApplicationName(string applicationName)
            {
                this.applicationName = applicationName;
                return this;
            }
            #endregion

            #region Parameters From Environment
            /// <summary>
            /// Target database name.
            /// </summary>
            /// <param name="database">database name</param>
            /// <returns>current builder</returns>
            public Builder DatabaseEnv(EnvironmentVariable database)
            {
                this.database = database;
                return this;
            }
            
            /// <summary>
            /// <para>
            /// Target database host address with or without port.
            /// </para>
            /// <i>
            ///  Examples:<br/>
            ///  ● localhost<br/> 
            ///  ● localhost:1433<br/> 
            ///  ● mssqlserver<br/> 
            ///  ● mssqlserver:1433<br/> 
            ///  ● https://dbaddress.azure.com:443
            /// </i>
            /// </summary>
            /// <param name="host">host dns or ip address with or without port</param>
            /// <returns>current builder</returns>
            public Builder HostEnv(EnvironmentVariable host)
            {
                this.host = host;
                return this;
            }

            /// <summary>
            /// Target port
            /// </summary>
            /// <param name="port"></param>
            /// <returns>current builder</returns>
            public Builder PortEnv(EnvironmentVariable port)
            {
                this.port = port;
                return this;
            }

            /// <summary>
            /// Database host authentication username.
            /// </summary>
            /// <param name="user">authentication username</param>
            /// <returns>current builder</returns>
            public Builder UserEnv(EnvironmentVariable user)
            {
                this.user = user;
                return this;
            }

            /// <summary>
            /// Database host authentication password.
            /// </summary>
            /// <param name="password">authentication password</param>
            /// <returns>current builder</returns>
            public Builder PasswordEnv(EnvironmentVariable password)
            {
                this.password = password;
                return this;
            }

            /// <summary>
            /// Attempt database connection timeout in seconds.
            /// </summary>
            /// <param name="timeoutInSec">database connection timeout in seconds</param>
            /// <returns>current builder</returns>
            public Builder TimeoutEnv(EnvironmentVariable timeoutInSec)
            {
                this.timeout = timeoutInSec;
                return this;
            }

            /// <summary>
            /// Database command timeout in seconds.
            /// </summary>
            /// <param name="commandTimeoutInSec">database command  timeout in seconds</param>
            /// <returns>current builder</returns>
            public Builder CommandTimeoutEnv(EnvironmentVariable commandTimeoutInSec)
            {
                this.commandTimeout = commandTimeoutInSec;
                return this;
            }

            /// <summary>
            /// Idle Database lifetime timeout in seconds.
            /// </summary>
            /// <param name="idleLifeTimeInSec">idle lifetime timeout in seconds</param>
            /// <returns>current builder</returns>
            public Builder IdleLifeTimeEnv(EnvironmentVariable idleLifeTimeInSec)
            {
                this.idleLifetime = idleLifeTimeInSec;
                return this;
            }

            /// <summary>
            /// Min pool size.
            /// </summary>
            /// <param name="minPoolSize">Min pool size</param>
            /// <returns>current builder</returns>
            public Builder MinPoolSizeEnv(EnvironmentVariable minPoolSize)
            {
                this.minPoolSize = minPoolSize;
                return this;
            }

            /// <summary>
            /// Max pool size.
            /// </summary>
            /// <param name="maxPoolSize">Max pool size</param>
            /// <returns>current builder</returns>
            public Builder MaxPoolSizeEnv(EnvironmentVariable maxPoolSize)
            {
                this.maxPoolSize = maxPoolSize;
                return this;
            }

            /// <summary>
            /// Some database can identify database connection for application name.
            /// </summary>
            /// <param name="applicationName">current application name</param>
            /// <returns>current builder</returns>
            public Builder ApplicationNameEnv(EnvironmentVariable applicationName)
            {
                this.applicationName = applicationName;
                return this;
            }
            #endregion

            #region Parameter Endpoint
            /// <summary>
            /// <para>
            /// Target database host address with or without port.
            /// </para>
            /// <i>
            ///  Examples:<br/>
            ///  ● https://dbaddress.azure.com:443
            /// </i>
            /// <para>
            /// <i>Obs.: You can use <see cref="Host(string)"/> or <see cref="HostEnv(EnvironmentVariable)"/> to do samething.</i>
            /// </para>
            /// </summary>
            /// <param name="accountEndpoint">host dns or ip address with or without port</param>
            /// <returns>current builder</returns>
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

            /// <summary>
            /// <para>
            /// For account endpoint connection, for example, to azure database connections.
            /// You can set an account key.
            /// </para>
            /// <para>
            /// <i>Obs.: You can use <see cref="Password(string)"/> or <see cref="PasswordEnv(EnvironmentVariable)"/> to do samething.</i>
            /// </para>
            /// </summary>
            /// <param name="accountKey">account key connection</param>
            /// <returns>current builder</returns>
            public Builder AccountKey(string accountKey)
            {
                this.password = accountKey;
                return this;
            }
            #endregion

            #region IDisposable
            /// <summary>
            /// Disposing builder parameters.
            /// </summary>
            protected override void OnDispose()
            {
                this.callbacks?.Clear();
                this.callbacks = null;
            }
            #endregion

            #region Build
            /// <summary>
            /// Build a context connection from builder parameters.
            /// </summary>
            /// <returns>new instance of context connection</returns>
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
