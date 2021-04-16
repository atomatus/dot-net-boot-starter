using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Service Collection for IConfigurationRoot and IConfiguration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Create an instance of <see cref="IConfigurationRoot"/> using
        /// appsettings.json and their overrides if present.
        /// Then add it as singletion service to current service collection.
        /// </summary>
        /// <param name="services">current service collection instance</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddConfigurationFromAppsettingsFile(this IServiceCollection services)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.development.json", true)
                .AddJsonFile("appsettings.dev.json", true)
                .AddJsonFile("appsettings.production.json", true)
                .AddJsonFile("appsettings.prod.json", true)
                .AddJsonFile("appsettings.override.json", true)
                .Build();

            return services
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IConfigurationRoot>(configuration);
        }
    }
}
