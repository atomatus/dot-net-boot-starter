﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context connection SqlServer extensions.
    /// </summary>
    public static class ContextConnectionSqlServerExtensions
    {
        private static bool OnBuildAsSqlServerCallback(ContextConnection.Builder builder, out ContextConnection conn)
        {
            conn = new ContextConnectionSqlServer(builder);
            return true;
        }

        /// <summary>
        /// Mark to build context connection as a SqlServer.
        /// <para>
        /// To load by ConnectionString in appsettings.json use the default sqlserver format connection
        /// string. Otherwise, inform connection credentials.
        /// </para>
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <returns>current builder</returns>
        public static ContextConnection.Builder AsSqlServer([NotNull] this ContextConnection.Builder builder)
        {
            return builder
                .AddDefaultConnectionStringOperation((b, c) => b.UseSqlServer(c))
                .AddBuildCallback(OnBuildAsSqlServerCallback);
        }

        /// <summary>
        /// Mark to build context connection as a SqlServer.
        /// <para>
        /// To load by ConnectionString in appsettings.json use the default sqlserver format connection
        /// string. Otherwise, inform connection credentials.
        /// </para>
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="provider">service provider</param>
        /// <param name="options">DbContext options builder</param>
        /// <returns>DbContext options builder</returns>
        public static DbContextOptionsBuilder AsSqlServerDbContextOptionsBuilder(
            [NotNull] this ContextConnection.Builder builder,
            [NotNull] IServiceProvider provider,
            [NotNull] DbContextOptionsBuilder options)
        {
            return builder
                .Configuration(provider)
                .AsSqlServer()
                .Build()
                .Attach(options);
        }

        /// <summary>
        /// <para>
        /// Registers the given context as a SqlServer's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its
        /// constructor. 
        /// </para>
        /// <para>
        /// Whether not set no one builder option using <paramref name="builderAction"/>
        /// will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, microsoft SQLServer use localhost, port 1433 and windows authentication.
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
        /// <param name="serviceAction">Can set dbcontext contract services as dependency injection</param>
        /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDbContextAsSqlServer<TContext>(
            [NotNull] this IServiceCollection services,
            [AllowNull] Action<ContextConnection.Builder> builderAction = null,
            [AllowNull] Action<IContextServiceCollection> serviceAction = null,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : ContextBase
        {
            return services.AddDbContextAs<TContext>(
                (builder, prov, opt) => builder.AsSqlServerDbContextOptionsBuilder(prov, opt),
                builderAction,
                serviceAction,
                contextLifetime,
                optionsLifetime);
        }

        /// <summary>
        /// <para>
        /// Register an dynamic context type as a SqlServer's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its 
        /// constructor. 
        /// </para>
        /// <para>
        /// Whether not set no one builder option using <paramref name="builderAction"/>
        /// will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, microsoft SQLServer use localhost, port 1433 and windows authentication.
        /// </para>
        /// <para>
        /// <i>
        /// When using this operation must be set the entity services
        /// by <paramref name="serviceAction"/> to be possible identify target entities for dbContext.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="services">Current service collection</param>
        /// <param name="builderAction">Can set builder options</param>
        /// <param name="serviceAction">Can set dbcontext contract services as dependency injection</param>
        /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDbContextAsSqlServer(
            [NotNull] this IServiceCollection services,
            [AllowNull] Action<ContextConnection.Builder> builderAction = null,
            [AllowNull] Action<IContextServiceCollection> serviceAction = null,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            return services.AddDbContextAs(
                (builder, prov, opt) => builder.AsSqlServerDbContextOptionsBuilder(prov, opt),
                builderAction,
                serviceAction,
                contextLifetime,
                optionsLifetime);
        }
    }
}
