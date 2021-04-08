using Com.Atomatus.Bootstarter.Context.Configuration;
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
        private class EntityConfigurationInternal<T> : EntityConfigurationBase<T>
            where T : class, IModel, IModelAltenateKey
        {
            protected override void OnConfigure(EntityTypeBuilder<T> builder) { }
        }

        private static Type TryGetGenericArgType(Type type, Type genericTypeDef)
        {
            return type.IsGenericType && 
                type.GetGenericTypeDefinition() == genericTypeDef ?
                type.GetGenericArguments().First() : null;
        }

        private void LoadEntityConfigurationsDefault(IEnumerable<Type> types, ModelBuilder modelBuilder)
        {
            var gconfigType = typeof(EntityConfigurationInternal<>);

            var gApplyConfigMethod = modelBuilder.GetType()
                .GetMethod(nameof(ModelBuilder.ApplyConfiguration));

            foreach(Type type in types)
            {
                var configType = gconfigType.MakeGenericType(type);
                var config = configType.GetConstructors().First().Invoke(null);

                var applyConfigMethod = gApplyConfigMethod.MakeGenericMethod(type);
                applyConfigMethod.Invoke(modelBuilder, new[] { config });
            }
        }

        private void AttemptToLoadEntityConfigurationFromDbSetDeclared(ModelBuilder modelBuilder)
        {
            if (!loadEntityConfigurationByEachDbSet)
            {
                return;
            }

            BindingFlags flags = BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetField |
                BindingFlags.GetProperty;

            var fields = this.GetType()
                .GetFields(flags)
                .Select(f => TryGetGenericArgType(f.FieldType, typeof(DbSet<>)));

            var props = this.GetType()
                .GetProperties(flags)
                .Select(p => TryGetGenericArgType(p.PropertyType, typeof(DbSet<>)));

            var groups = fields.Union(props).Where(t => t != null).GroupBy(t => t.Assembly);

            foreach (var g in groups)
            {
                var itens = g.ToList();
                modelBuilder.ApplyConfigurationsFromAssembly(g.Key, 
                    t => TryGetGenericArgType(t, typeof(IEntityTypeConfiguration<>)) is Type argType && 
                    itens.Remove(argType));
                LoadEntityConfigurationsDefault(itens, modelBuilder);
            }
        }

    }
}
