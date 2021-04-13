using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter.Services
{
    public abstract partial class ServiceCrud<TContext, TEntity, ID>
        where TEntity : ModelBase<ID>, new()
        where TContext : ContextBase
    {
        private static DbSet<T> GetOrCreateDbSet<T>(ContextBase context) where T : class, IModel
        {
            if(context is null)
            {
                return null;
            }

            var binding = BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetField |
                BindingFlags.GetProperty;

            static bool IsDbSetOfTargetEntity(Type type)
            {
                return type.IsGenericType &&
                       type.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                       type.GetGenericArguments().First() == typeof(T);
            }

            return context.GetType()
                .GetProperties(binding)
                .Where(p => IsDbSetOfTargetEntity(p.PropertyType))
                .Select(p => p.GetValue(context) as DbSet<T>)
                .FirstOrDefault() ??
                
                context.GetType()
                .GetFields(binding)
                .Where(f => IsDbSetOfTargetEntity(f.FieldType))
                .Select(p => p.GetValue(context) as DbSet<T>)
                .FirstOrDefault() ??
                
                context.Set<T>();
        }

        public ServiceCrud([NotNull] TContext context) : this(context, GetOrCreateDbSet<TEntity>(context)) { }
    }
}
