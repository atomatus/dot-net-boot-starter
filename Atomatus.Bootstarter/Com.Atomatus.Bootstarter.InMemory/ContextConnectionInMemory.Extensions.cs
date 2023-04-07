using Com.Atomatus.Bootstarter.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context connection InMemory extensions.
    /// </summary>
    public static class ContextConnectionInMemoryExtensions
    {
        private static bool OnBuildAsInMemoryCallback(ContextConnection.Builder builder, out ContextConnection conn)
        {
            conn = new ContextConnectionInMemory(builder);
            return true;
        }

        /// <summary>
        /// Mark to build context connection as a InMemory database.
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <returns>current builder</returns>
        public static ContextConnection.Builder AsInMemory([NotNull] this ContextConnection.Builder builder)
        {
            return builder
                .AddDefaultConnectionStringOperation((b, c) => b.UseInMemoryDatabase(c))
                .AddBuildCallback(OnBuildAsInMemoryCallback);
        }

        /// <summary>
        /// Mark to build context connection as a InMemory database.
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="provider">service provider</param>
        /// <param name="options">DbContext options builder</param>
        /// <returns>DbContext options builder</returns>
        public static DbContextOptionsBuilder AsInMemoryDbContextOptionsBuilder(
            [NotNull] this ContextConnection.Builder builder,
            [NotNull] IServiceProvider provider,
            [NotNull] DbContextOptionsBuilder options)
        {
            return builder
                .Configuration(provider)
                .AsInMemory()
                .Build()
                .Attach(options);
        }

        #region AddDbContextAs[InMemory] explicit DbContext
        /// <summary>
        /// <para>
        /// Registers the given context as a InMemory's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its
        /// constructor. 
        /// </para>
        /// <para>
        /// Whether not set no one builder option using <paramref name="builderAction"/>
        /// will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, InMemory load memory database.
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
        public static IServiceCollection AddDbContextAsInMemory<TContext>(
            [NotNull] this IServiceCollection services,
            [AllowNull] Action<ContextConnection.Builder> builderAction,
            [AllowNull] Action<IContextServiceCollection> serviceAction,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : ContextBase
        {
            return services.AddDbContextAs<TContext>(
                (builder, prov, opt) => builder.AsInMemoryDbContextOptionsBuilder(prov, opt),
                builderAction,
                serviceAction,
                contextLifetime,
                optionsLifetime);
        }

        /// <summary>
        /// <para>
        /// Registers the given context as a InMemory's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its
        /// constructor. 
        /// </para>
        /// <para>
        /// Whether not set no one builder option using <paramref name="builderAction"/>
        /// will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, InMemory load memory database.
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
        public static IServiceCollection AddDbContextAsInMemory<TContext>(
            [NotNull] this IServiceCollection services,
            [AllowNull] Action<ContextConnection.Builder> builderAction,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : ContextBase
        {
            return services.AddDbContextAsInMemory<TContext>(
                builderAction: builderAction,
                serviceAction: null,
                contextLifetime: contextLifetime,
                optionsLifetime: optionsLifetime);
        }

        /// <summary>
        /// <para>
        /// Registers the given context as a InMemory's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its
        /// constructor. 
        /// </para>
        /// <para>
        /// Will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, InMemory load memory database.
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
        /// <param name="serviceAction">Can set dbcontext contract services as dependency injection</param>
        /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDbContextAsInMemory<TContext>(
            [NotNull] this IServiceCollection services,
            [AllowNull] Action<IContextServiceCollection> serviceAction,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : ContextBase
        {
            return services.AddDbContextAsInMemory<TContext>(
                builderAction: null,
                serviceAction: serviceAction,
                contextLifetime: contextLifetime,
                optionsLifetime: optionsLifetime);
        }

        /// <summary>
        /// <para>
        /// Registers the given context as a InMemory's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its
        /// constructor. 
        /// </para>
        /// <para>
        /// Will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, InMemory load memory database.
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
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDbContextAsInMemory<TContext>(
            [NotNull] this IServiceCollection services) where TContext : ContextBase
        {
            return services.AddDbContextAsInMemory<TContext>(
                builderAction: null,
                serviceAction: null);
        }
        #endregion

        #region AddDbContextAs[InMemory] implicit and dynamic DbContext
        /// <summary>
        /// <para>
        /// Register an dynamic context type as a InMemory's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its
        /// constructor. 
        /// </para>
        /// <para>
        /// Whether not set no one builder option using <paramref name="builderAction"/>
        /// will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, InMemory load memory database.
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
        public static IServiceCollection AddDbContextAsInMemory(
            [NotNull] this IServiceCollection services,
            [AllowNull] Action<ContextConnection.Builder> builderAction,
            [AllowNull] Action<IContextServiceCollection> serviceAction,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            return services.AddDbContextAs(
                (builder, prov, opt) => builder.AsInMemoryDbContextOptionsBuilder(prov, opt),
                builderAction,
                serviceAction,
                contextLifetime,
                optionsLifetime);
        }

        /// <summary>
        /// <para>
        /// Register an dynamic context type as a InMemory's DbContext service in the Microsoft.Extensions.DependencyInjection.IServiceCollection.<br/>
        /// Use this method when using dependency injection in your application, such as
        /// with ASP.NET Core. For applications that don't use dependency injection, consider
        /// creating Microsoft.EntityFrameworkCore.DbContext instances directly with its
        /// constructor. 
        /// </para>
        /// <para>
        /// Will try to load from appsettings.json looking for "ConnectionString[s]" key,
        /// if not found, will try to load using default database connection configurations.<br/>
        /// For example, InMemory load memory database.
        /// </para>
        /// <para>
        /// <i>
        /// When using this operation must be set the entity services
        /// by <paramref name="serviceAction"/> to be possible identify target entities for dbContext.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="services">Current service collection</param>        
        /// <param name="serviceAction">Can set dbcontext contract services as dependency injection</param>
        /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDbContextAsInMemory(
            [NotNull] this IServiceCollection services,
            [AllowNull] Action<IContextServiceCollection> serviceAction,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            return services.AddDbContextAsInMemory(
                builderAction: null,
                serviceAction: serviceAction,
                contextLifetime: contextLifetime,
                optionsLifetime: optionsLifetime);
        }
        #endregion

    }
}
