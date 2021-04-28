using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Context.Ensures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter
{
    internal sealed class DynamicTypeContext : DynamicType
    {
        private Type cType;
        private IEnumerable<Type> entities;
        private bool disposed;

        private readonly bool disposeAfterRequest;
        
        ~DynamicTypeContext()
        {
            this.Dispose(false);
        }

        internal DynamicTypeContext() : base() 
        {
            this.cType              = typeof(ContextBase);
            this.entities           = Enumerable.Empty<Type>();
            this.disposeAfterRequest = false;
        }

        private DynamicTypeContext(DynamicTypeContext other, Type cType = null, IEnumerable<Type> entities = null)
        {
            this.dictionary = other.dictionary;
            this.cType = cType ?? other.cType;
            this.entities = entities ?? other.entities;
            this.disposeAfterRequest = true;
        }

        private void Dispose(bool _)
        {
            this.cType = null;
            this.entities = null;
            this.dictionary = null;
            this.disposed = true;
        }

        private void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private Type CreateType(Key key)
        {
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

        internal Type GetOrCreate()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(DynamicTypeContext));
            }

            try
            {
                Key key = new Key(entities.Union(new[] { cType }));
                return this.GetOrAdd(key, k => CreateType(k));
            }
            finally
            {
                if (disposeAfterRequest)
                {
                    this.Dispose();
                }
            }
        }

        internal DynamicTypeContext Ensures(ContextConnectionParameters parameters)
        {
            bool hasMigrate = parameters.ensureMigrate.HasValue && parameters.ensureMigrate.Value;
            bool hasCreated = !parameters.ensureCreated.HasValue || parameters.ensureCreated.Value;
            bool hasDeletedOnDispose = parameters.ensureDeletedOnDispose.HasValue && parameters.ensureDeletedOnDispose.Value;
            
            var cType =

                hasMigrate ?
                    typeof(ContextBaseForEnsureMigration) :

                hasCreated ?
                    hasDeletedOnDispose ? typeof(ContextBaseForEnsureCreationAndDeletedOnDispose) :
                    typeof(ContextBaseForEnsureCreation) :

                hasDeletedOnDispose ? typeof(ContextBaseForEnsureDeletedOnDispose) :
                    null;

            return new DynamicTypeContext(this, cType: cType);
        }

        internal DynamicTypeContext Entities(IEnumerable<Type> entities)
        {
            return new DynamicTypeContext(this, entities: entities);
        }

        internal DynamicTypeContext Entities(IContextServiceTypes services)
        {
            return Entities(services
                .Select(st => st.GetIServiceGenericArgument())
                .Distinct());
        }
    }
}
