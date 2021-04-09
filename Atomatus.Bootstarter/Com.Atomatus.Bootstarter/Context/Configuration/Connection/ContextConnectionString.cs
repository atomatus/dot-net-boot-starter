﻿using System;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Context.Configuration.Connection
{
    internal sealed class ContextConnectionString : ContextConnection
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

                return configuration
                    ?.GetSection("ConnectionStrings")
                    ?.GetChildren()
                    ?.FirstOrDefault()
                    ?.Value;
            }
            else
            {
                key = key.StartsWith("ConnectionStrings:") ? key : "ConnectionStrings:" + key;
                return configuration is null ? null : configuration[key] ??
                    throw new InvalidOperationException($"ConnectionString key ({connectionStringKey}) not found in appsettings.json!");
            }
        }

        protected override string GetConnectionString() => connectionString;

        internal bool IsValid() => !string.IsNullOrWhiteSpace(connectionString);
    }
}
