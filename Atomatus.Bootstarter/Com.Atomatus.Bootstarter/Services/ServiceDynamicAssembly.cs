using Com.Atomatus.Bootstarter.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Com.Atomatus.Bootstarter.Services
{
    internal sealed class ServiceDynamicAssembly
    {
        private class Key : IEqualityComparer<Key>
        {
            private readonly Guid guid;
            private readonly int hashCode;

            public Key(params Type[] args) : this(args.Select(t => t.GUID).ToArray()) { }

            public Key(params Guid[] args)
            {
                this.hashCode = Objects.GetHashCode(args);
                this.guid = args.Aggregate(Merge);
            }

            private Guid Merge(Guid curr, Guid next)
            {
                byte[] cArr = curr.ToByteArray();
                byte[] nArr = next.ToByteArray();

                for(int i=0, c = 16 /*Guid len*/; i < c; i++)
                {
                    cArr[i] ^= nArr[i];
                }

                return new Guid(cArr);
            }

            public bool Equals(Key x, Key y) => x == y || x.hashCode == y.hashCode;

            public int GetHashCode(Key obj) => obj.hashCode;

            public override int GetHashCode() => hashCode;

            public override bool Equals(object obj) => obj is Key other && Equals(this, other);

            public override string ToString() => guid.ToString();
        }

        private readonly ConcurrentDictionary<Key, Type> dictionary;

        private static readonly ServiceDynamicAssembly instance;

        public static ServiceDynamicAssembly GetInstance() => instance;

        static ServiceDynamicAssembly()
        {
            instance = new ServiceDynamicAssembly();
        }

        private ServiceDynamicAssembly()
        {
            dictionary = new ConcurrentDictionary<Key, Type>();
        }

        private Type CreateType(Key key, Type cType, Type sType)
        {
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

            //create a new type from serviceCrudImpl to TContext, TEntity and ID.
            //implementing interface TService.            

            var moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName(cType.GUID.ToString()), AssemblyBuilderAccess.Run)
                .DefineDynamicModule($"{key}.Services");

            var typeBuilder = moduleBuilder
                .DefineType($"ServiceCrudImpl{eType.Name}To{cType.Name}", 
                    TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class, 
                    servImplType /*parent*/, 
                    new[] { sType } /*interface*/);

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
            else if (cType.IsNotPublic)
            {
                throw new AccessViolationException($"Context \"{cType.Name}\" class is not public, " +
                    $"then is not possible use it to generate a service runtime type!");
            }
            else if (sType.IsNotPublic)
            {
                throw new AccessViolationException($"Service \"{sType.Name}\" interface is not public," +
                    $"then is not possible use it to generate a service runtime type!");
            }

            Key key = new Key(cType, sType);
            return dictionary.GetOrAdd(key, k => CreateType(k, cType, sType));
        }
    }
}
