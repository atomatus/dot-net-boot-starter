using Com.Atomatus.Bootstarter.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Service extensions.
    /// </summary>
    public static class ServiceExtensions
    {
        private static readonly AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString());

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
            Type cType = typeof(TContext); //context type.
            Type sType = typeof(TService); //service type.

            if (cType.IsAbstract)
            {
                throw new ArgumentException($"Context type \"{cType.Name}\" " +
                    $"can not be abstract!");
            }
            else if (!sType.IsInterface)
            {
                throw new ArgumentException($"Service type \"{sType.Name}\" " +
                    $"is not an interface!");
            }

            //recover IService<,> interface type
            Type genSType = sType.GetGenericInterfaceType(typeof(IService<,>)) ??
                throw new ArgumentException($"Service type \"{sType.Name}\" " +
                    $"does not implements {typeof(IService<>)}, " +
                    $"therefore is impossible identify entity and id type!");

            Type eType = genSType.GetGenericArguments().First(); //entity type.
            Type iType = genSType.GetGenericArguments().Last(); //entity id type.

            //generate a type informing parameters to ServiceCrudImpl<,,>
            Type genServImplType    = typeof(ServiceCrud<,,>);
            Type servImplType       = genServImplType.MakeGenericType(cType, eType, iType);

            //the ServiceCrudImpl constructor.
            var servImplTypeCtor = servImplType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance, null,
                new[] { cType }, null);

            //create a new type from serviceCrudImpl to TContext and TEntity, ID
            //implementing interface TService.            
            var typeBuilder         = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)                
                .DefineDynamicModule($"Services")
                .DefineType($"ServiceCrudImpl{eType.Name}To{cType.Name}", TypeAttributes.Public, servImplType);

            //add interface to new type definition
            typeBuilder.AddInterfaceImplementation(sType);

            //create a constructor from base type
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public, 
                CallingConventions.Standard | CallingConventions.HasThis, 
                new[] { cType });

            //set constructor operation to call base type constructor
            var ilGenerator = ctorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0); // push "this"
            ilGenerator.Emit(OpCodes.Ldarg_1); // push the TContext parameter to base type
            ilGenerator.Emit(OpCodes.Call, servImplTypeCtor); // call base constructor
            ilGenerator.Emit(OpCodes.Nop); // C# compiler add 2 NOPS, so
            ilGenerator.Emit(OpCodes.Nop); // we'll add them, too.
            ilGenerator.Emit(OpCodes.Ret); //return

            //build the new dynamic type
            Type sFinalType = typeBuilder.CreateType();

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
