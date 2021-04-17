using Com.Atomatus.Bootstarter.Context;
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
        /// <summary>
        /// Create a dynamic service type to specified service type (<typeparamref name="TService"/>)
        /// and target context <typeparamref name="TContext"/>. 
        /// Then, invoke callback to register it in service collection.
        /// </summary>
        /// <typeparam name="TContext">database context type</typeparam>
        /// <typeparam name="TService">service interface type</typeparam>
        /// <param name="addServiceCallback">service collection callback operation</param>
        /// <returns>current service collection</returns>
        private static IServiceCollection CreateAndAddServiceDynamicType<TContext, TService>([NotNull] Func<Type, Type, IServiceCollection> addServiceCallback)
            where TContext : ContextBase
            where TService : IService
        {
            var sFinalType  = ServiceDynamicAssembly.GetInstance().GetOrCreateType<TContext, TService>();
            var sType       = typeof(TService);
            return addServiceCallback.Invoke(sType, sFinalType);
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
        public static IServiceCollection AddService<TContext, TService>([NotNull] this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TContext : ContextBase
            where TService : IService
        {
            return serviceLifetime switch
            {
                ServiceLifetime.Singleton   => CreateAndAddServiceDynamicType<TContext, TService>(services.AddSingleton),
                ServiceLifetime.Scoped      => CreateAndAddServiceDynamicType<TContext, TService>(services.AddScoped),
                ServiceLifetime.Transient   => CreateAndAddServiceDynamicType<TContext, TService>(services.AddTransient),
                _ => throw new NotImplementedException(),
            };
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
            return CreateAndAddServiceDynamicType<TContext, TService>(services.AddScoped);
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
            return CreateAndAddServiceDynamicType<TContext, TService>(services.AddSingleton);
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
            return CreateAndAddServiceDynamicType<TContext, TService>(services.AddTransient);
        }
    }
}
