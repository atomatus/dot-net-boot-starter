using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Service Collection for IConfigurationRoot and IConfiguration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static IConfigurationBuilder AddJsonFileOrEmbeddedResource(this IConfigurationBuilder builder, string path, bool optional = true)
        {
            if (EmbeddedResource.TryLoad(path, out EmbeddedResource res))
            {
                return builder.AddJsonStream(res.GetStream());
            }
            else
            {
                return builder.AddJsonFile(path, optional);
            }
        }

        /// <summary>
        /// <para>
        /// Create an instance of <see cref="IConfigurationRoot"/> using
        /// appsettings.json and their overrides if present.
        /// Then add it as singleton service to current service collection.
        /// </para>
        /// <para>
        /// <i>
        /// The appsettings.json and their overrides, how like, appsettings.development.json,<br/>
        /// may be an EmbeddedResource or a copy to output directory 
        /// (to do it, click over file with mouse right button and navigate to properties).
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="services">current service collection instance</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddConfigurationFromAppsettingsFile([NotNull] this IServiceCollection services)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFileOrEmbeddedResource("appsettings.json", false)
                .AddJsonFileOrEmbeddedResource("appsettings.development.json")
                .AddJsonFileOrEmbeddedResource("appsettings.dev.json")
                .AddJsonFileOrEmbeddedResource("appsettings.production.json")
                .AddJsonFileOrEmbeddedResource("appsettings.prod.json")
                .AddJsonFileOrEmbeddedResource("appsettings.override.json")
                .Build();

            return services
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IConfigurationRoot>(configuration);
        }
    }
}
