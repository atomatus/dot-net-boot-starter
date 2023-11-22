using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Cosmos, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c51a009f369b01e2686a4cb3d5330d7610cacc2db1c6b8692c745162831f2809286f15ce0bff54a0f24c35f0f498ae59059ae05e78aa4f2b36c4b75ea03c6de0b5696ae1ed6024cb1a2139e8d4c6a50bfcfec4f651e2411f66c123078bfe8d58ff21e5021462011188759a9b35ec1feee26137c41ec11a67037b993c41fb8bad")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.InMemory, PublicKey=002400000480000094000000060200000024000052534131000400000100010069d12aef39583dd25d176ceda75428d275396c001f4d65d0e6381bdb6f842d2d35c5873085800abaa74777c9368419704f6cb07a0da061f986d03d93c86fee27be0c161702ee5a48b77e67f8006033e064ef077e157e1b7e109568c02cac9f03329e9f3d9a7e1127333ad5385eb98cb97bd393b08848a6520da610b99f516dd0")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Postgres, PublicKey=002400000480000094000000060200000024000052534131000400000100010081883cc4a796c90d1d02a587f661e5ce61a7bc27c2780f8df196d3462290fc6fcc25cf6d539defb830264c657d1f8af10565d32ae101e70135911176e3ed2a9370dce6a10457e630736002e278ca656561b6a8afa890c9133ba190cf8b1dd553cf77afeb24f5638fd567b1cb7b3ec4850735e13fdfe6f8058ac58ae4f255ebb6")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlite, PublicKey=002400000480000094000000060200000024000052534131000400000100010021edf2cdd0575c9fa2558015f7a3538fb57f029679b9caed7499c1d6607d6646d61fd9d6937b597ed2dad30f3f9c407d7360b4b007722d3de11bb0f645ea958bc7c9b4d390349487fa753526d957938a86c82af14754adad03a8fe0097e1aa807b4709d433e33b714973b37b048b88d2b25bb2db33eb7ae03732f0fccc65f794")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlserver, PublicKey=002400000480000094000000060200000024000052534131000400000100010079ca8479c5a52b5b284eff5e26078cea9e5916c0b506ed2cca26012d01cdc1d0636d835a1992394f726d792ea7e23b8da849a9e530b7a837731ca5a5a112fcdf4db8448d505bb66dbc687b486252be73366f7747325775503bab93b1829d763e4cd0c82ad4443d8c4eedaf534bffd42fed1f12923288754c012385e6422f47d8")]
namespace Com.Atomatus.Bootstarter.Context
{
    public abstract partial class ContextConnection : ContextConnectionParameters
    {
        internal delegate bool TryBuildContextConnectionCallback(Builder builder, out ContextConnection conn);
        internal delegate void OptionsBuilderCallback(object optionsBuilder);
           
        /// <summary>
        /// Context connection builder.
        /// </summary>
        public sealed class Builder : ContextConnectionParameters
        {
            #region Constructor
            internal Builder() { }

            internal Builder(Builder other) : base(other) { }
            #endregion

            #region Local Parameters
            private event TryBuildContextConnectionCallback Callbacks;
            private event OptionsBuilderCallback OptionsBuilderCallbacks;

            internal Builder AddBuildCallback(TryBuildContextConnectionCallback callback)
            {
                this.Callbacks -= callback;
                this.Callbacks += callback;
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

            internal Builder AddOptionsCallback(OptionsBuilderCallback optionAction)
            {
                this.OptionsBuilderCallbacks -= optionAction;
                this.OptionsBuilderCallbacks += optionAction;
                return this;
            }

            internal void InvokeOptions(object options)
            {
                this.OptionsBuilderCallbacks?.Invoke(options);
            }
            #endregion

            #region Parameters
            internal Builder Database<TContext>() where TContext : ContextBase
            {
                static string Replace(string target, string value) =>
                    target.Replace(value, string.Empty,
                    StringComparison.CurrentCultureIgnoreCase);                
                string name = Replace(Replace(typeof(TContext).Name, "Context"), "db").ToLower() + "db";
                return Database(name).DenyDynamicContext<TContext>();
            }

            /// <summary>
            /// Target database name.
            /// </summary>
            /// <param name="database">database name</param>
            /// <returns>current builder</returns>
            public Builder Database(string database)
            {
                this.database = database;
                this.allowNoDatabase = false;
                return this;
            }

            /// <summary>
            /// Set up no one database target.
            /// <para>
            /// <i>
            /// Use this configuration when connection is restricted by
            /// procedures and you don't know dartabase name or details for example.
            /// </i>
            /// </para>
            /// </summary>
            /// <returns>current builder</returns>
            public Builder NoDatabase()
            {
                this.allowNoDatabase = true;
                this.database = null;
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

            /// <summary>
            /// Database connection as readonly.
            /// </summary>
            /// <returns>current builder</returns>
            public Builder ReadOnly()
            {
                this.readOnly = true;
                return this;
            }

            /// <summary>
            /// Database cache as shared.
            /// </summary>
            /// <returns>current builder</returns>
            public Builder SharedCache()
            {
                this.sharedCache = true;
                return this;
            }

            /// <summary>
            /// Connection authentication by SO (Windows).
            /// <para>
            /// This options is setup by default true when does not set <see cref="User(string)"/> and <see cref="Password(string)"/>.
            /// </para>
            /// </summary>
            /// <param name="integratedSecurity">true, enable integrated security, otherwise false</param>
            /// <returns>current builder</returns>
            public Builder IntegratedSecurity(bool integratedSecurity = true)
            {
                this.integratedSecurity = integratedSecurity;
                return this;
            }

            /// <summary>
            /// <para>
            /// <b>MultipleActiveResultSets (MARS)</b> is a feature in Microsoft SQL Server exclusive, that allows multiple active result sets
            /// (queries) to be processed concurrently on a single connection.
            /// Normally, without MARS, a single SQL Server connection can only process one active result set at a time.
            /// </para>
            /// <para>
            /// This means that if one query is in progress, other queries must wait until the first one completes before they can be executed.
            /// </para>
            /// </summary>
            /// <param name="multipleActiveResultSets">true, enable MARS, otherwise false</param>
            /// <returns>current builder</returns>
            public Builder MultipleActiveResultSets(bool multipleActiveResultSets = true)
            {
                this.multipleActiveResultSets = multipleActiveResultSets;
                return this;
            }

            /// <summary>
            /// Set up current server connection as encrypted.
            /// <para>
            /// <i>Warn: You must to check server Trust Certification when using it.</i>
            /// </para>
            /// </summary>
            /// <param name="encrypt">true, enabled, otherwise false.</param>
            /// <returns>current builder</returns>
            public Builder Encrypt(bool encrypt = true)
            {
                this.encrypt = encrypt;
                return this;
            }

            /// <summary>
            /// Set up current server encrypted connection to accept server certified as reliabled (trusted).
            /// </summary>
            /// <param name="trustServerCertificate">true, enabled, otherwise false.</param>
            /// <returns>current builder</returns>
            public Builder TrustServerCertificate(bool trustServerCertificate = true)
            {
                this.trustServerCertificate = trustServerCertificate;
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

            #region Parameter Database Creation Rules
            private void RequireGrantDynamicContext()
            {
                if (!this.grantDynamicContext)
                {
                    throw new InvalidCastException("Attempt to define Creation or Deletion rule for Explicit Context" +
                        (string.IsNullOrEmpty(contextName) ? "" : $" ({contextName})") + ".\r\n" +
                        "Unfortunately it is not allowed.\r\n" +
                        "For an explicit context you must setup all creation rules" +
                        "(Context.Database.EnsureCreated(), Context.Database.EnsureDeleted(), Context.Database.Migrate(), and more) inner it or explicitly.");
                }
            }

            private Builder DenyDynamicContext<TContext>()
            {
                this.contextName = typeof(TContext).Name;
                this.grantDynamicContext = false;
                return this;
            }

            internal Builder GrantDynamicContext()
            {
                this.grantDynamicContext = true;
                return this;
            }

            /// <summary>
            /// <para>
            /// Ensures that the database for the context exists. If it exists, no action is
            /// taken. If it does not exist then the database and all its schema are created.
            /// If the database exists, then no effort is made to ensure it is compatible with
            /// the model for this context.
            /// </para>
            /// <para>
            /// <i>
            /// Note that this API does not use migrations to create the database. In addition,
            /// the database that is created cannot be later updated using migrations. If you
            /// are targeting a relational database and using migrations, you can use the <see cref="EnsureMigrate"/>
            /// method to ensure the database is created and all migrations are applied.
            /// </i>
            /// </para>
            /// <para>
            /// <i>
            /// Warning: This request only works for dynamic ContextBase creation, therefore
            /// when called for typed context it will not work.
            /// </i>
            /// </para>
            /// </summary>
            /// <returns>current builder</returns>
            /// <exception cref="InvalidOperationException">
            /// Throws exception in attempt of define creation or deletion rule out of dynamic context builder.
            /// Therefore, trying to define rule in explicit context. For a contextBase (DBContext) 
            /// explicitly defined, set all (ContextBase.Database.EnsureCreated(), ContextBase.Database.EnsureDeleted(), 
            /// ContextBase.Database.Migrate(),...) rules inner it.
            /// </exception>
            public Builder EnsureCreated()
            {
                this.RequireGrantDynamicContext();
                this.ensureCreated = true;
                return this;
            }

            /// <summary>
            /// Ensures that the database for the context does not exist. If it does not exist,
            /// no action is taken. If it does exist then the database is deleted 
            /// when context is disposed.
            /// <para>
            /// <i>
            /// Warning: The entire database is deleted, and no effort is made to remove just
            /// the database objects that are used by the model for this context.
            /// </i>
            /// </para>
            /// <para>
            /// <i>
            /// Warning: This request only works for dynamic ContextBase creation, therefore
            /// when called for typed context it will not work.
            /// </i>
            /// </para>
            /// </summary>
            /// <returns>current builder</returns>
            /// <exception cref="InvalidOperationException">
            /// Throws exception in attempt of define creation or deletion rule out of dynamic context builder.
            /// Therefore, trying to define rule in explicit context. For a contextBase (DBContext) 
            /// explicitly defined, set all (ContextBase.Database.EnsureCreated(), ContextBase.Database.EnsureDeleted(), 
            /// ContextBase.Database.Migrate(),...) rules inner it.
            /// </exception>
            public Builder EnsureDeletedOnDispose()
            {
                this.RequireGrantDynamicContext();
                this.ensureCreated ??= false;
                this.ensureDeletedOnDispose = true;
                return this;
            }

            /// <summary>
            /// <para>
            /// Ensures that applies any pending migrations for the context to the database.<br/>
            /// Will create the database if it does not already exist.
            /// </para>
            /// <para>
            /// <i>
            /// Note that this API is mutually exclusive with <see cref="EnsureCreated"/>.
            /// EnsureCreated does not use migrations to create the database and therefore the
            /// database that is created cannot be later updated using migrations.
            /// </i>
            /// </para>
            /// <para>
            /// <i>
            /// Warning: This request only works for dynamic ContextBase creation, therefore
            /// when called for typed context it will not work.
            /// </i>
            /// </para>
            /// </summary>
            /// <returns>current builder</returns>
            /// <exception cref="InvalidOperationException">
            /// Throws exception in attempt of define creation or deletion rule out of dynamic context builder.
            /// Therefore, trying to define rule in explicit context. For a contextBase (DBContext) 
            /// explicitly defined, set all (ContextBase.Database.EnsureCreated(), ContextBase.Database.EnsureDeleted(), 
            /// ContextBase.Database.Migrate(),...) rules inner it.
            /// </exception>
            public Builder EnsureMigrate()
            {
                this.RequireGrantDynamicContext();
                this.ensureMigrate = true;
                this.ensureCreated = false;
                this.ensureDeletedOnDispose = false;
                return this;
            }
            #endregion

            #region IDisposable
            /// <summary>
            /// Disposing builder parameters.
            /// </summary>
            protected override void OnDispose()
            {
                this.Callbacks = null;
                this.OptionsBuilderCallbacks = null;
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
                    if(this.Callbacks != null)
                    {
                        foreach (var d in this.Callbacks.GetInvocationList())
                        {
                            if (d is TryBuildContextConnectionCallback callback &&
                                callback.Invoke(this, out ContextConnection conn))
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
