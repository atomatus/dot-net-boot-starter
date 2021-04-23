using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter.Context.Configuration
{
    internal static class EntityTypeConfigurationReflection
    {
        private class EntityTypeConfigurationInternal<TEntity, ID> : EntityTypeConfigurationBase<TEntity, ID>
               where TEntity : class, IModel<ID>
        {
            private readonly Type entityType;
            private readonly MethodInfo configureMethod;

            public EntityTypeConfigurationInternal(Type entityType)
            {
                this.entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
                this.configureMethod = TryGetConfigureMethodDeclared(entityType);
            }

            private MethodInfo TryGetConfigureMethodDeclared(Type type)
            {
                Type getbType = typeof(EntityTypeBuilder<>).MakeGenericType(type);
                Binder binder = Type.DefaultBinder;
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                return
                    type.GetMethod("Configure", flags, binder, new Type[] { getbType }, null) ??                    
                    type.GetMethod("OnConfigure", flags, binder, new Type[] { getbType }, null);
            }

            protected override void OnConfigure(EntityTypeBuilder<TEntity> builder) 
            {
                //indicating that Entity IModel type contains
                //Configure or OnConfigure declaration,
                //then request it.
                if(this.configureMethod != null)
                {
                    var obj = entityType.GetConstructors().First().Invoke(null);
                    configureMethod.Invoke(obj, new[] { builder });
                }
            }
        }

        public static void ApplyConfigurationToEntities(ModelBuilder modelBuilder, params Type[] types)
        {
            ApplyConfigurationToEntities(modelBuilder, types.ToList());
        }

        public static void ApplyConfigurationToEntities(ModelBuilder modelBuilder, IEnumerable<Type> types)
        {
            var gconfigType = typeof(EntityTypeConfigurationInternal<,>);
            var applyConfigMethod = typeof(ModelBuilder).GetMethod(nameof(ModelBuilder.ApplyConfiguration));

            foreach (Type type in types)
            {
                if (type.GetGenericInterfaceType(typeof(IModel<>))
                    ?.GetGenericArguments()
                    ?.FirstOrDefault() is Type idType)
                {
                    var configType = gconfigType.MakeGenericType(type, idType);
                    var config = configType.GetConstructors().First().Invoke(new[] { type });

                    applyConfigMethod
                        .MakeGenericMethod(type)
                        .Invoke(modelBuilder, new[] { config });
                }
            }
        }
    }
}
