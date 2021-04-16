using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter.Context
{
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

        protected ContextConnection(Builder builder) : base(builder) { }

        protected abstract string GetConnectionString();

        protected internal abstract DbContextOptionsBuilder Attach(DbContextOptionsBuilder options);
        
        protected bool HasUsername() => !string.IsNullOrWhiteSpace(user);

        protected bool HasPassword() => !string.IsNullOrWhiteSpace(password);

        protected bool HasUsernameAndPassword() => HasUsername() && HasPassword();

        protected bool HasNotUsernameAndPassword() => !HasUsername() && !HasPassword();

        protected bool HasIdleLifetime() => idleLifetime > 0;

        protected bool MinPoolSize() => minPoolSize > 0;

        protected bool MaxPoolSize() => maxPoolSize > 0;

        public sealed override string ToString()
        {
            return GetConnectionString();
        }

        public static implicit operator string(ContextConnection context)
        {
            return context.GetConnectionString();
        }
    }
}
