using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Context.Ensures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Com.Atomatus.Bootstarter
{
    internal sealed class DynamicTypeContext : DynamicType
    {
        private Type cType;

        internal DynamicTypeContext() : base() { }

        private Type CreateType(Key key, IEnumerable<Type> entities)
        {
            this.cType ??= typeof(ContextBase);
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

        internal DynamicTypeContext Ensures(ContextConnectionParameters parameters)
        {
            bool hasMigrate = parameters.ensureMigrate.HasValue && parameters.ensureMigrate.Value;
            bool hasCreated = !parameters.ensureCreated.HasValue || parameters.ensureCreated.Value;
            bool hasDeletedOnDispose = parameters.ensureDeletedOnDispose.HasValue && parameters.ensureDeletedOnDispose.Value;
            
            this.cType =

                hasMigrate ?
                    typeof(ContextBaseForEnsureMigration) :

                hasCreated ?
                    hasDeletedOnDispose ? typeof(ContextBaseForEnsureCreationAndDeletedOnDispose) :
                    typeof(ContextBaseForEnsureCreation) :

                hasDeletedOnDispose ? typeof(ContextBaseForEnsureDeletedOnDispose) :
                    null;

            return this;
        }
    }
}
