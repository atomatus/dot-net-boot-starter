using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter.Context
{
    public abstract partial class ContextBase
    {
        private class EntityConfigurationInternal<TEntity, ID> : EntityConfigurationBase<TEntity, ID>
            where TEntity : class, IModel<ID>
        {
            protected override void OnConfigure(EntityTypeBuilder<TEntity> builder) { }
        }

        private static bool CheckIsEntityTypeConfiguration(Type type, out Type entityType)
        {
            Type aux1  = type.GetGenericInterfaceType(typeof(IEntityTypeConfiguration<>));
            entityType = aux1?.GetGenericArguments().FirstOrDefault();
            return entityType != default;
        }

        private void LoadEntityConfigurationsDefault(IEnumerable<Type> types, ModelBuilder modelBuilder)
        {
            var gconfigType = typeof(EntityConfigurationInternal<,>);

            var gApplyConfigMethod = modelBuilder.GetType()
                .GetMethod(nameof(ModelBuilder.ApplyConfiguration));

            foreach(Type type in types)
            {
                Type idType = typeof(IModel).IsAssignableFrom(type) ?
                    (type.GetProperty(nameof(IModel<int>.Id), BindingFlags.Instance | BindingFlags.Public)
                        ?.PropertyType ??
                    type.GetField(nameof(IModel<int>.Id), BindingFlags.Instance | BindingFlags.Public)
                        ?.FieldType) : null;

                if (idType is null)
                {
                    continue;
                }

                var configType = gconfigType.MakeGenericType(type, idType);
                var config = configType.GetConstructors().First().Invoke(null);

                var applyConfigMethod = gApplyConfigMethod.MakeGenericMethod(type);
                applyConfigMethod.Invoke(modelBuilder, new[] { config });
            }
        }
        
        private void AttemptLoadEntityConfigurationsDeclaredToDbSetDeclared(ModelBuilder modelBuilder)
        {
            if (!loadEntityConfigurationByEachDbSet)
            {
                return;
            }

            //Looking for DBSet definitions from current context instance and group it by your assemblies
            BindingFlags flags = BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetField |
                BindingFlags.GetProperty;

            var fields = this.GetType()
                .GetFields(flags)
                .Where(f => f.FieldType.IsSubclassOfRawGenericType(typeof(DbSet<>)))
                .Select(f => f.FieldType.GetGenericArguments().First());

            var props = this.GetType()
                .GetProperties(flags)
                .Where(p => p.PropertyType.IsSubclassOfRawGenericType(typeof(DbSet<>)))
                .Select(p => p.PropertyType.GetGenericArguments().First());

            var groups = fields
                .Union(props)
                .Where(t => t != null)
                .GroupBy(t => t.Assembly);

            foreach (var g in groups)
            {
                var itens = g.ToList();

                //attempt to find classes generated implementing IEntityTypeConfiguration.
                //when found, remove the entity from list and load it configuation to current context.
                modelBuilder.ApplyConfigurationsFromAssembly(g.Key, 
                    t => CheckIsEntityTypeConfiguration(t, out Type entityType) && 
                    itens.Remove(entityType));

                //attempt to create and load minimum default configuration
                //to entities than does not contains explit IEntityConfiguration using EntityConfigurationInternal.
                LoadEntityConfigurationsDefault(itens, modelBuilder);
            }
        }
    }
}
