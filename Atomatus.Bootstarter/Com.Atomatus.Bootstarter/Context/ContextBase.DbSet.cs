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

        /// <summary>
        /// Looking for <see cref="DbSet{TEntity}"/> declared how field or property (pref. it),
        /// if not found, create a <see cref="DbSet{TEntity}"/> and store it to future usage
        /// in same context, when context is disposed stored dbset is released too.
        /// </summary>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <returns>entity dbset that can be used to query and save instances of TEntity.</returns>
        protected internal DbSet<TEntity> GetOrSet<TEntity>() where TEntity : class, IModel
        {
            if(dbSetDic.TryGetValue(typeof(TEntity), out object dbSet))
            {
                return dbSet as DbSet<TEntity>;
            }

            var binding = BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetField |
                BindingFlags.DeclaredOnly |
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

        /// <summary>
        /// Releases the allocated resources for this context.
        /// </summary>
        public override void Dispose()
        {
            this.DetachAllEntities();
            this.DisposeDBSetDictionary();
            base.Dispose();
        }

        /// <summary>
        ///  Releases the allocated resources for this context.
        /// </summary>
        /// <returns></returns>
        public override ValueTask DisposeAsync()
        {
            this.DetachAllEntities();
            this.DisposeDBSetDictionary();
            return base.DisposeAsync();
        }
    }
}
