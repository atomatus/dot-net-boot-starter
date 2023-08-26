using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context connection parameters.
    /// </summary>
    public abstract class ContextConnectionParameters : IDisposable
    {
        #region Fields and Delegates
        /// <summary>
        /// Connection string callback.
        /// </summary>
        /// <param name="builder">database context options builder</param>
        /// <param name="connectionString">database connection string</param>
        /// <returns></returns>
        protected internal delegate DbContextOptionsBuilder ConnectionStringFunction(DbContextOptionsBuilder builder, string connectionString);

        /// <summary>
        /// Database name.
        /// </summary>
        protected string database;

        /// <summary>
        /// Host address/endpoint.
        /// </summary>
        protected string host;

        /// <summary>
        /// Host port
        /// </summary>
        protected int port;

        /// <summary>
        /// Authentication username.
        /// </summary>
        protected string user;

        /// <summary>
        /// Authentication password.
        /// </summary>
        protected string password;

        /// <summary>
        /// Database connection timeout in seconds.
        /// </summary>
        protected int timeout;

        /// <summary>
        /// Database command timeout in seconds.
        /// </summary>
        protected int commandTimeout;

        /// <summary>
        /// Idle lifetime in seconds.
        /// </summary>
        protected int idleLifetime;

        /// <summary>
        /// Min pool size.
        /// </summary>
        protected int minPoolSize;

        /// <summary>
        /// Max pool size.
        /// </summary>
        protected int maxPoolSize;

        /// <summary>
        /// Database connection as readOnly.
        /// </summary>
        protected bool readOnly;

        /// <summary>
        /// Database as shared mode.
        /// </summary>
        protected bool sharedCache;

        /// <summary>
        /// Database make usage of OS (Windows) authentication (when user and password is not setup explicitly).
        /// </summary>
        protected bool? integratedSecurity;

        /// <summary>
        /// Enable multiple active result sets (MARS), allow multiple queries on the same connection.
        /// </summary>
        protected bool? multipleActiveResultSets;

        /// <summary>
        /// Database connetion must be encrypted.
        /// </summary>
        protected bool? encrypt;

        /// <summary>
        /// Server certified must be reliable in encrypted connection (<see cref="encrypt"/> = true).
        /// </summary>
        protected bool? trustServerCertificate;

        /// <summary>
        /// Application name.
        /// </summary>
        protected string applicationName;

        /// <summary>
        /// Application configuration properties.
        /// </summary>
        protected IConfiguration configuration;

        /// <summary>
        /// connection string key to be used to
        /// find connection string from <see cref="configuration"/>.
        /// </summary>
        protected string connectionStringKey;

        /// <summary>
        /// Connection string callback.
        /// </summary>
        protected ConnectionStringFunction connectionStringCallback;

        /// <summary>
        /// Grant dynamic context rules.
        /// </summary>
        protected bool grantDynamicContext;

        /// <summary>
        /// Explicit context name.
        /// </summary>
        protected string contextName;
        #endregion

        #region Creation Ensures fields
        /// <summary>
        /// Ensures that the database for the target context exists,
        /// if the database does not exists will be created.
        /// </summary>
        internal bool? ensureCreated;

        /// <summary>
        /// Ensures that the database for the target context will be
        /// deleted on context dispose.
        /// </summary>
        internal bool? ensureDeletedOnDispose;

        /// <summary>
        /// Ensures that the database for the target context will request Migrate operations.
        /// </summary>
        internal bool? ensureMigrate;
        #endregion

        #region Constructor/Deconstructor
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ContextConnectionParameters()
        {
            this.Dispose();
        }

        /// <summary>
        /// constructor copying parameters from another object
        /// </summary>
        /// <param name="other"></param>
        protected ContextConnectionParameters(ContextConnectionParameters other)
        {
            this.database                   = other.database;
            this.host                       = other.host;
            this.port                       = other.port;
            this.user                       = other.user;
            this.password                   = other.password;
            this.timeout                    = other.timeout;
            this.commandTimeout             = other.commandTimeout;
            this.idleLifetime               = other.idleLifetime;
            this.minPoolSize                = other.minPoolSize;
            this.maxPoolSize                = other.maxPoolSize;
            this.applicationName            = other.applicationName;
            this.readOnly                   = other.readOnly;
            this.sharedCache                = other.sharedCache;
            this.integratedSecurity         = other.integratedSecurity;
            this.multipleActiveResultSets   = other.multipleActiveResultSets;
            this.encrypt                    = other.encrypt;
            this.trustServerCertificate     = other.trustServerCertificate;
            this.configuration              = other.configuration;
            this.connectionStringKey        = other.connectionStringKey;
            this.connectionStringCallback   = other.connectionStringCallback;
            this.ensureCreated              = other.ensureCreated;
            this.ensureDeletedOnDispose     = other.ensureDeletedOnDispose;
            this.ensureMigrate              = other.ensureMigrate;
            this.grantDynamicContext        = other.grantDynamicContext;
            this.contextName                = other.contextName;
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        protected ContextConnectionParameters() { }
        #endregion

        #region IDisposable
        /// <summary>
        /// Fired when disposing object.
        /// </summary>
        protected virtual void OnDispose() { }

        private void Dispose()
        {
            this.OnDispose();
            this.database                   = null;
            this.host                       = null;
            this.user                       = null;
            this.password                   = null;
            this.applicationName            = null;
            this.configuration              = null;
            this.connectionStringKey        = null;
            this.connectionStringCallback   = null;
            this.ensureCreated              = null;
            this.ensureDeletedOnDispose     = null;
            this.ensureMigrate              = null;
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
