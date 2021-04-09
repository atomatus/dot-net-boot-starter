using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Com.Atomatus.Bootstarter.Context.Configuration.Connection
{
    public static class ContextConnectionStringExtensions
    {
        private static bool OnBuildFromConnectionStringCallback(ContextConnection.Builder builder, out ContextConnection conn)
        {
            var aux = new ContextConnectionString(builder);
            bool isValid = aux.IsValid();
            conn = isValid ? aux : default;
            return isValid;
        }

        public static ContextConnection.Builder Configuration(this ContextConnection.Builder builder, IConfiguration configuration)
        {
            builder.configuration = configuration;
            builder.AddBuildCallback(OnBuildFromConnectionStringCallback);
            return builder;
        }

        public static ContextConnection.Builder Configuration(this ContextConnection.Builder builder, IServiceProvider provider)
        {
            return builder.Configuration((provider?.GetService<IConfiguration>() ??
                 throw new ArgumentNullException(nameof(provider))) ??
                 throw new ArgumentException("Service provider is do not attaching IConfiguration!"));
        }

        public static ContextConnection.Builder ConnectionStringKey(this ContextConnection.Builder builder, string connectionStringKey)
        {
            builder.connectionStringKey = connectionStringKey;
            return builder;
        }
    }
}
