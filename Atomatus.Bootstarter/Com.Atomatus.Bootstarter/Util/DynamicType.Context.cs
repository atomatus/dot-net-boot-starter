using Com.Atomatus.Bootstarter.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Com.Atomatus.Bootstarter
{
    internal sealed class DynamicTypeContext : DynamicType
    {
        internal DynamicTypeContext() : base() { }

        private Type CreateType(Key key, IEnumerable<Type> entities)
        {
            Type cType = typeof(ContextBase);
            Type oType = typeof(DbContextOptions);

            AssemblyName aname  = GetAssemblyName(cType);
            var moduleBuilder   = GetModuleBuilder(aname, key, "Context");
            var typeBuilder     = GetTypeBuilder(
                builder: moduleBuilder,
                name: $"LocalContext",
                parent: cType);

            Type dbsetGenType = typeof(DbSet<>);

            foreach(Type eType in entities)
            {
                string name = eType.Name;
                Type dbsetType = dbsetGenType.MakeGenericType(eType);
                SetProperty(typeBuilder, dbsetType, name);                
            }

            SetConstructorSingleArgumentAndCallBase(typeBuilder, cType, oType);

            //build the new dynamic type
            Type sFinalType = typeBuilder.CreateType();
            return sFinalType;
        }

        internal Type GetOrCreate(IEnumerable<Type> entities)
        {
            Key key = new Key(entities);
            return this.GetOrAdd(key, k => CreateType(k, entities));
        }
    }
}
