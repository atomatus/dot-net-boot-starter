
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context connection postgres extensions.
    /// </summary>
    public static class ContextConnectionPostgresExtensions
    {
        private static bool OnBuildAsPostgresCallback(ContextConnection.Builder builder, out ContextConnection conn)
        {
            conn = new ContextConnectionPostgres(builder);
            return true;
        }

        /// <summary>
        /// Mark to build context connection as a PostgresSQL.
        /// <para>
        /// To load by ConnectionString in appsettings.json use the default postgressql format connection
        /// string. Otherwise, inform connection credentials.
        /// </para>
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <returns>current builder</returns>
        public static ContextConnection.Builder AsPostgres([NotNull] this ContextConnection.Builder builder)
        {
            return builder
                .AddDefaultConnectionStringOperation((b, c) => b.UseNpgsql(o => o.SetPostgresVersion(9, 6)).UseNpgsql(c))
                .AddBuildCallback(OnBuildAsPostgresCallback);
        }

        /// <summary>
        /// Mark to build context connection as a PostgresSQL.
        /// <para>
        /// To load by ConnectionString in appsettings.json use the default postgressql format connection
        /// string. Otherwise, inform connection credentials.
        /// </para>
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="provider">service provider</param>
        /// <param name="options">DbContext options builder</param>
        /// <returns>DbContext options builder</returns>
        public static DbContextOptionsBuilder AsPostgresDbContextOptionsBuilder(
            [NotNull] this ContextConnection.Builder builder,
            [NotNull] IServiceProvider provider,
            [NotNull] DbContextOptionsBuilder options)
        {
            return builder
                .Configuration(provider)
                .AsPostgres()
                .Build()
                .Attach(options);
        }

        /// <summary>
        /// <para>
        /// Registers the given context as a PostgresSQL's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its
        /// constructor. 
        /// </para>
        /// <para>
        /// Whether not set no one builder option using <paramref name="builderAction"/>
        /// will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, PostgresSQL use localhost, default port 5432, no authentication and connection timeout of 120 seconds.
        /// </para>
        /// <para>
        /// The Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)
        /// method can then be overridden to configure a connection string and other options.
        /// For more information on how to use this method, see the Entity Framework Core
        /// documentation at https://aka.ms/efdocs. 
        /// </para>
        /// <para>
        /// For more information on using dependency
        /// injection, see https://go.microsoft.com/fwlink/?LinkId=526890.
        /// This overload has an optionsAction that provides the application's System.IServiceProvider.
        /// </para>
        /// <seealso cref="EntityFrameworkServiceCollectionExtensions.AddDbContext{TContextService, TContextImplementation}(IServiceCollection, Action{IServiceProvider, DbContextOptionsBuilder}, ServiceLifetime, ServiceLifetime)"/>
        /// </summary>
        /// <typeparam name="TContext">Context target</typeparam>
        /// <param name="services">Current service collection</param>
        /// <param name="builderAction">Can set builder options</param>
        /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDbContextAsPostgres<TContext>(
            [NotNull] this IServiceCollection services,
            [AllowNull] Action<ContextConnection.Builder> builderAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : ContextBase
        {
            var builder = new ContextConnection.Builder().Database<TContext>();
            builderAction?.Invoke(builder);
            return services.AddDbContext<TContext>(
                optionsAction: (prov, opt) => builder.AsPostgresDbContextOptionsBuilder(prov, opt),
                contextLifetime: contextLifetime,
                optionsLifetime: optionsLifetime);
        }
    }
}
