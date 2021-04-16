using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Context
{
    internal class ContextConnectionString : ContextConnection
    {
        private readonly string connectionString;

        public ContextConnectionString(Builder builder) : base(builder) 
        {
            connectionString = GetConnectionStringInternal();
        }

        private string GetConnectionStringInternal()
        {
            string key = this.connectionStringKey;

            if (string.IsNullOrWhiteSpace(key))
            {
                if (DotnetRunningInContainer)
                {
                    return null;//Key not set and app is running in container, preferences to environment config.
                }

                return (
                    configuration
                        ?.GetSection("ConnectionStrings") ??                    
                    configuration
                        ?.GetSection("ConnectionString"))
                    ?.GetChildren()
                    ?.FirstOrDefault()
                    ?.Value;
            }
            else
            {
                return configuration is null ? null : 
                    configuration[key] ??
                    configuration[(key.Contains(':') ? key : "ConnectionStrings:" + key)] ??
                    throw new InvalidOperationException($"ConnectionString key ({connectionStringKey}) not found in appsettings.json!");
            }
        }

        protected override string GetConnectionString() => connectionString;

        protected internal override DbContextOptionsBuilder Attach(DbContextOptionsBuilder options)
        {
            return connectionStringCallback?.Invoke(options, connectionString) ?? options;
        }

        internal bool IsValid() => !string.IsNullOrWhiteSpace(connectionString);
    }
}
