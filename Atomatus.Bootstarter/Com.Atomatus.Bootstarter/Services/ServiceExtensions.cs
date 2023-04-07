using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Service extensions.
    /// </summary>
    public static class ServiceExtensions
    {
        #region IService
        /// <summary>
        /// Create a dynamic service type to specified service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, invoke callback to register it in service collection.
        /// </summary>
        /// <typeparam name="TContext">database context type</typeparam>
        /// <typeparam name="TService">service contract type</typeparam>
        /// <param name="addServiceCallback">service collection callback operation</param>
        /// <param name="implementedType">implemented type generated to target context and service contract type</param>
        /// <returns>current service collection</returns>
        private static IServiceCollection CreateAndAddServiceDynamicType<TContext, TService>([NotNull] Func<Type, Type, IServiceCollection> addServiceCallback, ref Type implementedType)
            where TContext : ContextBase
            where TService : IService
        {
            implementedType ??= DynamicTypeFactory.AsService().GetOrCreateType<TContext, TService>();
            var sType         = typeof(TService);
            return addServiceCallback.Invoke(sType, implementedType);
        }

        /// <summary>
        /// Create a dynamic service type to specified service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how <paramref name="serviceLifetime"/> defined it.
        /// </summary>
        /// <typeparam name="TContext">database context type.</typeparam>
        /// <typeparam name="TService">service interface type.</typeparam>
        /// <param name="services">current service collection.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the Service in the container.</param>
        /// <param name="implementedType">implemented type generated to target context and service contract type, or null to request generate a new value</param>
        /// <returns>current service collection</returns>
        internal static IServiceCollection AddService<TContext, TService>([NotNull] this IServiceCollection services, ServiceLifetime serviceLifetime, ref Type implementedType)
            where TContext : ContextBase
            where TService : IService
        {
            return serviceLifetime switch
            {
                ServiceLifetime.Singleton   => CreateAndAddServiceDynamicType<TContext, TService>(services.AddSingleton, ref implementedType),
                ServiceLifetime.Scoped      => CreateAndAddServiceDynamicType<TContext, TService>(services.AddScoped, ref implementedType),
                ServiceLifetime.Transient   => CreateAndAddServiceDynamicType<TContext, TService>(services.AddTransient, ref implementedType),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Create a dynamic service type to specified service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how <paramref name="serviceLifetime"/> defined it.
        /// </summary>
        /// <typeparam name="TContext">database context type.</typeparam>
        /// <typeparam name="TService">service interface type.</typeparam>
        /// <param name="services">current service collection.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the Service in the container.</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddService<TContext, TService>([NotNull] this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TContext : ContextBase
            where TService : IService
        {
            Type aux = null;
            return services.AddService<TContext, TService>(serviceLifetime, ref aux);
        }

        /// <summary>
        /// Create a dynamic service type to specified interface service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how scoped service type.
        /// </summary>
        /// <typeparam name="TContext">database context type</typeparam>
        /// <typeparam name="TService">service interface type</typeparam>
        /// <param name="services">current service collection</param>
        /// <param name="implementedType">implemented type generated to target context and service contract type, or null to request generate a new value</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceScoped<TContext, TService>([NotNull] this IServiceCollection services, ref Type implementedType)
            where TContext : ContextBase
            where TService : IService
        {
            return CreateAndAddServiceDynamicType<TContext, TService>(services.AddScoped, ref implementedType);
        }

        /// <summary>
        /// Create a dynamic service type to specified interface service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how scoped service type.
        /// </summary>
        /// <typeparam name="TContext">database context type</typeparam>
        /// <typeparam name="TService">service interface type</typeparam>
        /// <param name="services">current service collection</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceScoped<TContext, TService>([NotNull] this IServiceCollection services)
            where TContext : ContextBase
            where TService : IService
        {
            Type aux = null;
            return services.AddServiceScoped<TContext, TService>(ref aux);
        }

        /// <summary>
        /// Create a dynamic service type to specified service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how singleton service type.
        /// </summary>
        /// <typeparam name="TContext">database context type</typeparam>
        /// <typeparam name="TService">service interface type</typeparam>
        /// <param name="services">current service collection</param>
        /// <param name="implementedType">implemented type generated to target context and service contract type, or null to request generate a new value</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceSingleton<TContext, TService>([NotNull] this IServiceCollection services, ref Type implementedType)
            where TContext : ContextBase
            where TService : IService
        {
            return CreateAndAddServiceDynamicType<TContext, TService>(services.AddSingleton, ref implementedType);
        }

        /// <summary>
        /// Create a dynamic service type to specified service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how singleton service type.
        /// </summary>
        /// <typeparam name="TContext">database context type</typeparam>
        /// <typeparam name="TService">service interface type</typeparam>
        /// <param name="services">current service collection</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceSingleton<TContext, TService>([NotNull] this IServiceCollection services)
            where TContext : ContextBase
            where TService : IService
        {
            Type aux = null;
            return services.AddServiceSingleton<TContext, TService>(ref aux);
        }

        /// <summary>
        /// Create a dynamic service type to specified service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how transient service type.
        /// </summary>
        /// <typeparam name="TContext">database context type</typeparam>
        /// <typeparam name="TService">service interface type</typeparam>
        /// <param name="services">current service collection</param>
        /// <param name="implementedType">implemented type generated to target context and service contract type, or null to request generate a new value</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceTransient<TContext, TService>([NotNull] this IServiceCollection services, ref Type implementedType)
            where TContext : ContextBase
            where TService : IService
        {
            return CreateAndAddServiceDynamicType<TContext, TService>(services.AddTransient, ref implementedType);
        }

        /// <summary>
        /// Create a dynamic service type to specified service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how transient service type.
        /// </summary>
        /// <typeparam name="TContext">database context type</typeparam>
        /// <typeparam name="TService">service interface type</typeparam>
        /// <param name="services">current service collection</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceTransient<TContext, TService>([NotNull] this IServiceCollection services)
            where TContext : ContextBase
            where TService : IService
        {
            Type aux = null;
            return services.AddServiceTransient<TContext, TService>(ref aux);
        }

        /// <summary>
        /// Adds a service of the type specified in <typeparamref name="TImplementation"/> name="TService" /> with an
        /// implementation of the type specified in <typeparamref name="TImplementation"/> to the
        /// specified <see cref="IServiceCollection"/> and his <paramref name="lifetime" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="lifetime">specify current service instance mode</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddServiceImpl<TService, TImplementation>([NotNull] this IServiceCollection services, [NotNull] ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            return lifetime switch
            {
                ServiceLifetime.Scoped => services.AddScoped<TService, TImplementation>(),
                ServiceLifetime.Singleton => services.AddSingleton<TService, TImplementation>(),
                ServiceLifetime.Transient => services.AddTransient<TService, TImplementation>(),
                _ => throw new NotImplementedException($"Service lifetime not implemented: {lifetime}"),
            };
        }
        #endregion

        #region IModel
        /// <summary>
        /// Create a dynamic service CRUD type (<see cref="IServiceCrud{TEntity, ID}"/>) to specified model type (<typeparamref name="TModel"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how <paramref name="serviceLifetime"/> defined it.
        /// </summary>
        /// <typeparam name="TContext">database context type.</typeparam>
        /// <typeparam name="TModel">model type.</typeparam>
        /// <typeparam name="TID">id model type.</typeparam>
        /// <param name="services">current service collection.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the Service in the container.</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddService<TContext, TModel, TID>([NotNull] this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TContext : ContextBase
            where TModel : IModel<TID>
        {
            Type implemenedType = null;
            return services
                .AddService<TContext, IServiceCrud<TModel, TID>>(serviceLifetime, ref implemenedType)
                .AddService<TContext, IServiceCrud<TModel>>(serviceLifetime, ref implemenedType)
                .AddService<TContext, IServiceCrudAsync<TModel, TID>>(serviceLifetime, ref implemenedType);
        }

        /// <summary>
        /// Create a dynamic service CRUD type (<see cref="IServiceCrud{TEntity, ID}"/>) to specified model type (<typeparamref name="TModel"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how scoped service type.
        /// </summary>
        /// <typeparam name="TContext">database context type.</typeparam>
        /// <typeparam name="TModel">model type.</typeparam>
        /// <typeparam name="TID">id model type.</typeparam>
        /// <param name="services">current service collection</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceScoped<TContext, TModel, TID>([NotNull] this IServiceCollection services)
            where TContext : ContextBase
            where TModel : IModel<TID>
        {
            return services.AddService<TContext, TModel, TID>(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Create a dynamic service CRUD type (<see cref="IServiceCrud{TEntity, ID}"/>) to specified model type (<typeparamref name="TModel"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how singleton service type.
        /// </summary>
        /// <typeparam name="TContext">database context type.</typeparam>
        /// <typeparam name="TModel">model type.</typeparam>
        /// <typeparam name="TID">id model type.</typeparam>
        /// <param name="services">current service collection</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceSingleton<TContext, TModel, TID>([NotNull] this IServiceCollection services)
            where TContext : ContextBase
            where TModel : IModel<TID>
        {
            return services.AddService<TContext, TModel, TID>(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Create a dynamic service CRUD type (<see cref="IServiceCrud{TEntity, ID}"/>) to specified model type (<typeparamref name="TModel"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, register it to service collection how transient service type.
        /// </summary>
        /// <typeparam name="TContext">database context type.</typeparam>
        /// <typeparam name="TModel">model type.</typeparam>
        /// <typeparam name="TID">id model type.</typeparam>
        /// <param name="services">current service collection</param>
        /// <returns>current service collection</returns>
        public static IServiceCollection AddServiceTransient<TContext, TModel, TID>([NotNull] this IServiceCollection services)
            where TContext : ContextBase
            where TModel : IModel<TID>
        {
            return services.AddService<TContext, TModel, TID>(ServiceLifetime.Transient);
        }
        #endregion
    }
}
