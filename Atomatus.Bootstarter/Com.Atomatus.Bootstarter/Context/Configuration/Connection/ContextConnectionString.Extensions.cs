using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Cosmos")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Postgres")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlite")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlserver")]
namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context Connection extensions for context string
    /// </summary>
    internal static class ContextConnectionStringExtensions
    {
        private static bool OnBuildFromConnectionStringCallback(ContextConnection.Builder builder, out ContextConnection conn)
        {
            var aux = new ContextConnectionString(builder);
            bool isValid = aux.IsValid();
            conn = isValid ? aux : default;
            return isValid;
        }

        /// <summary>
        /// Define configuration and add callback to build it 
        /// as connection string explicit for defined database structure in appseting.json.
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="configuration">configuration values</param>
        /// <returns>current buider</returns>
        public static ContextConnection.Builder Configuration(this ContextConnection.Builder builder, IConfiguration configuration)
        {
            return builder
                .AddConfiguration(configuration)
                .AddBuildCallback(OnBuildFromConnectionStringCallback);
        }

        /// <summary>
        /// Define configuration from service provider and add callback to build it 
        /// as connection string explicit for defined database structure in appseting.json.
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="provider">service provider that contains IConfiguration</param>
        /// <returns>current builder</returns>
        public static ContextConnection.Builder Configuration(this ContextConnection.Builder builder, IServiceProvider provider)
        {
            return builder.Configuration((provider != null ? provider.GetService<IConfiguration>() :
                 throw new ArgumentNullException(nameof(provider), "Service provider can not be null!")) ??
                 throw new ArgumentException("Service provider is do not attaching IConfiguration!"));
        }

        /// <summary>
        /// Define connection string key to recovery connection string from configurations (appsetings.json).
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="connectionStringKey">connection string access key</param>
        /// <returns>current builder</returns>
        public static ContextConnection.Builder ConnectionStringKey(this ContextConnection.Builder builder, string connectionStringKey)
        {
            return builder
                .AddConnectionStringKey(connectionStringKey)
                .AddBuildCallback(OnBuildFromConnectionStringCallback);
        }
    }
}
