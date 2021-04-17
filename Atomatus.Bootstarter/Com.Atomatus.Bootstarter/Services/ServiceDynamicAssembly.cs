using Com.Atomatus.Bootstarter.Context;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Com.Atomatus.Bootstarter.Services
{
    internal sealed class ServiceDynamicAssembly
    {
        private readonly ConcurrentDictionary<Guid, Type> dictionary;

        private static readonly ServiceDynamicAssembly instance;

        public static ServiceDynamicAssembly GetInstance() => instance;

        static ServiceDynamicAssembly()
        {
            instance = new ServiceDynamicAssembly();
        }

        private ServiceDynamicAssembly()
        {
            dictionary = new ConcurrentDictionary<Guid, Type>();
        }

        private Type CreateType(Guid key, Type cType, Type sType)
        {
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
            Type genServImplType = typeof(ServiceCrud<,,>);
            Type servImplType = genServImplType.MakeGenericType(cType, eType, iType);

            //the ServiceCrudImpl constructor.
            var servImplTypeCtor = servImplType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance, null,
                new[] { cType }, null);

            //create a new type from serviceCrudImpl to TContext and TEntity, ID
            //implementing interface TService.            
            var typeBuilder = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName(key.ToString()), AssemblyBuilderAccess.Run)
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
            return sFinalType;
        }

        public Type GetOrCreateType<TContext, TService>()
            where TContext : ContextBase
            where TService : IService
        {
            Type cType = typeof(TContext); //context type.
            Type sType = typeof(TService); //service type.

            return dictionary.GetOrAdd(cType.GUID, (key) => CreateType(key, cType, sType));
        }
    }
}
