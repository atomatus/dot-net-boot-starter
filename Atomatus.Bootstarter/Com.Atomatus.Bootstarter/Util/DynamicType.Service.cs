using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Services;
using System;
using System.Reflection;

namespace Com.Atomatus.Bootstarter
{
    internal sealed class DynamicTypeService : DynamicType
    {
        internal DynamicTypeService() : base() { }

        private Type CreateType(Key key, Type cType, Type sType)
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
            else if (sType.IsNotPublic)
            {
                throw new AccessViolationException($"Service \"{sType.Name}\" interface is not public," +
                    $"then is not possible use it to generate a service runtime type!");
            }

            //recover IService<,> interface type
            sType.GetIServiceGenericArguments(out Type eType /*entity type*/ , out Type iType /*idType*/);

            //generate a type informing parameters to ServiceCrudImpl<,,>
            Type dbcType = cType.IsPublic ? cType : cType.BaseType ?? typeof(ContextBase);//context type
            Type genServImplType = typeof(ServiceCrud<,,>);
            Type servImplType = genServImplType.MakeGenericType(dbcType, eType, iType);

            //create a new type from serviceCrudImpl to TContext, TEntity and ID.
            //implementing interface TService.            

            AssemblyName aname  = GetAssemblyName(cType);
            var moduleBuilder   = GetModuleBuilder(aname, key, "Services");
            var typeBuilder     = GetTypeBuilder(
                builder: moduleBuilder,
                name: $"ServiceCrudImpl{eType.Name}To{cType.Name}",
                parent: servImplType,
                interfaces: sType);

            SetConstructorSingleArgumentAndCallBase(typeBuilder, servImplType, cType);

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
            Key key = new Key(cType, sType);
            return this.GetOrAdd(key, k => CreateType(k, cType, sType));
        }
    }

}
