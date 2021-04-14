using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Context
{
    public abstract partial class ContextBase
    {
        private readonly ConcurrentDictionary<Type, object> dbSetDic;
        
        ContextBase()
        {
            dbSetDic = new ConcurrentDictionary<Type, object>();
        }

        protected internal DbSet<TEntity> GetOrSet<TEntity>() where TEntity : class, IModel
        {
            if(dbSetDic.TryGetValue(typeof(TEntity), out object dbSet))
            {
                return dbSet as DbSet<TEntity>;
            }

            var binding = BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetField |
                BindingFlags.GetProperty;

            static bool IsDbSetOfTargetEntity(Type type)
            {
                return type.IsGenericType &&
                       type.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                       type.GetGenericArguments().First() == typeof(TEntity);
            }

            return this.GetType()
                .GetProperties(binding)
                .Where(p => IsDbSetOfTargetEntity(p.PropertyType))
                .Select(p => p.GetValue(this) as DbSet<TEntity>)
                .FirstOrDefault() ??

                this.GetType()
                .GetFields(binding)
                .Where(f => IsDbSetOfTargetEntity(f.FieldType))
                .Select(p => p.GetValue(this) as DbSet<TEntity>)
                .FirstOrDefault() ??

                dbSetDic.GetOrAdd(typeof(TEntity), (t) => this.Set<TEntity>()) as DbSet<TEntity>;
        }

        private void DisposeDBSetDictionary()
        {
            dbSetDic.Clear();
        }

        public override void Dispose()
        {
            this.DetachAllEntities();
            this.DisposeDBSetDictionary();
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            this.DetachAllEntities();
            this.DisposeDBSetDictionary();
            return base.DisposeAsync();
        }
    }
}
