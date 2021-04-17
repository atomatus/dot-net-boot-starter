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
            this.configuration              = other.configuration;
            this.connectionStringKey        = other.connectionStringKey;
            this.connectionStringCallback   = other.connectionStringCallback;
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
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
