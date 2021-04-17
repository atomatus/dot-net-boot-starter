using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Represents a structural context connection to
    /// generate the connection string to be used on
    /// <see cref="DbContextOptionsBuilder"/> to build it and connect to target database.
    /// </summary>
    public abstract partial class ContextConnection
    {
        /// <summary>
        /// Environment variable to identify whether current app is running in container.
        /// </summary>
        protected static readonly EnvironmentVariable DotnetRunningInContainer;

        static ContextConnection()
        {
            DotnetRunningInContainer = "DOTNET_RUNNING_IN_CONTAINER";
        }

        /// <summary>
        /// Conext connection constructor.
        /// </summary>
        /// <param name="builder">builder parameters</param>
        protected ContextConnection(Builder builder) : base(builder) { }

        /// <summary>
        /// Generate connect string or explicit connection string defined
        /// in appsettings.json recovery by <see cref="Builder.AddConnectionStringKey(string)"/> defined.
        /// </summary>
        /// <returns>connection string or null value</returns>
        protected abstract string GetConnectionString();

        /// <summary>
        /// Attach current context connection to database context options builder.
        /// </summary>
        /// <param name="options">target builder</param>
        /// <returns>target builder</returns>
        protected internal abstract DbContextOptionsBuilder Attach(DbContextOptionsBuilder options);
        
        /// <summary>
        /// Check whether username to database connection was set.
        /// </summary>
        /// <returns></returns>
        protected bool HasUsername() => !string.IsNullOrWhiteSpace(user);

        /// <summary>
        /// Check whether password to database connection was set.
        /// </summary>
        /// <returns></returns>
        protected bool HasPassword() => !string.IsNullOrWhiteSpace(password);

        /// <summary>
        /// Check whether username and password to database connection was set.
        /// </summary>
        /// <returns></returns>
        protected bool HasUsernameAndPassword() => HasUsername() && HasPassword();

        /// <summary>
        /// Check whether username and password is not set to database connection.
        /// </summary>
        /// <returns></returns>
        protected bool HasNotUsernameAndPassword() => !HasUsername() && !HasPassword();

        /// <summary>
        /// Check whether idle lifetime to database connection was set.
        /// </summary>
        /// <returns></returns>
        protected bool HasIdleLifetime() => idleLifetime > 0;

        /// <summary>
        /// Check whether min pool size to database connection was set.
        /// </summary>
        /// <returns></returns>
        protected bool MinPoolSize() => minPoolSize > 0;

        /// <summary>
        /// Check whether max pool size to database connection was set.
        /// </summary>
        /// <returns></returns>
        protected bool MaxPoolSize() => maxPoolSize > 0;

        /// <summary>
        /// Get explicit defined or generate connection string.
        /// </summary>
        /// <returns></returns>
        public sealed override string ToString()
        {
            return GetConnectionString();
        }

        /// <summary>
        /// Convert current context connection to connection string.
        /// </summary>
        /// <param name="context"></param>
        public static implicit operator string(ContextConnection context)
        {
            return context.GetConnectionString();
        }
    }
}
