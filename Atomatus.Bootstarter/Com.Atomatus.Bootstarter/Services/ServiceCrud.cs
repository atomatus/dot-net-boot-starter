using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Abstract Service Crud.
    /// </summary>
    /// <typeparam name="TContext">target DbContext</typeparam>
    /// <typeparam name="TEntity">target entity</typeparam>
    /// <typeparam name="ID">target entity id</typeparam>
    public abstract partial class ServiceCrud<TContext, TEntity, ID> : IServiceCrud<TEntity, ID>
        where TEntity : class, IModel<ID>, new()
        where TContext : ContextBase
    {
        /// <summary>
        /// DbContext instance.
        /// </summary>
        protected readonly TContext dbContext;

        /// <summary>
        /// DbSet generated it or recovered from <see cref="dbContext"/> property definition.
        /// </summary>
        protected readonly DbSet<TEntity> dbSet;

        /// <summary>
        /// Constructor for dbcontext and dbset.
        /// </summary>
        /// <param name="context">target dbcontext.</param>
        /// <param name="dbSet">target dbset.</param>
        public ServiceCrud([NotNull] TContext context, [NotNull] DbSet<TEntity> dbSet)
        {
            this.dbContext = context ?? throw new ArgumentNullException(nameof(context));
            this.dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        }

        /// <summary>
        /// Constructor for dbcontext and
        /// autodiscovery dbset.
        /// </summary>
        /// <param name="context">target dbcontext.</param>
        public ServiceCrud([NotNull] TContext context) : this(context, context?.GetOrSet<TEntity>()) { }

        #region [C]reate
        /// <summary>
        /// Insert a new value to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <returns>entity within ids</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        public TEntity Save(TEntity entity)
        {
            OnBeforeInsertCallback(entity);
            RequireValidate(entity);
            dbSet.Add(entity ?? throw new ArgumentNullException(nameof(entity)));

            try
            {
                dbContext.SaveChanges();
                OnInsertedCallback(entity);
            }
            finally
            {
                dbContext.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }
        #endregion

        #region [R]ead        
        private static void RequireEntityImplementIModelAlternateKey()
        {
            if (!typeof(IModelAltenateKey).IsAssignableFrom(typeof(TEntity)))
            {
                throw new InvalidCastException($"Is not possible manipulate, find or delete " +
                    $"value of Entity \"{typeof(TEntity).Name}\" using Uuid, because " +
                    $"this class does not implements {typeof(IModelAltenateKey).Name} contract!");
            }
        }

        /// <summary>
        /// Check whether current ID exists on persistence base.<br/>
        /// </summary>
        /// <param name="id">primary key</param>
        /// <returns>true, value exists, otherwhise false</returns>
        public bool Exists(ID id)
        {
            return dbSet
                .AsNoTracking()
                .Any(e => e.Id.Equals(id));
        }

        /// <summary>
        /// Check if where condition makes matches with some data.
        /// </summary>
        /// <param name="whereCondition">where condition</param>
        /// <returns>true if any elements in the source sequence pass the test in the specified predicate; otherwise, false.</returns>
        public bool Exists(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .AsNoTracking()
                .Any(whereCondition);
        }

        /// <summary>
        /// Check whether current uuid exists on persistence base.<br/>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </summary>
        /// <param name="uuid">alternate key uuid</param>
        /// <returns>true, value exists, otherwhise false</returns>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public bool ExistsByUuid(Guid uuid)
        {
            RequireEntityImplementIModelAlternateKey();
            return dbSet
                .AsNoTracking()
                .OfType<IModelAltenateKey>()
                .Any(e => e.Uuid == uuid);
        }

        /// <summary>
        /// Check whether current entity exists on persistence base.
        /// </summary>
        /// <param name="e">target entity</param>
        /// <returns>true, value exists, otherwhise false</returns>
        public bool Exists(TEntity e)
        {
            if (e is IModelAltenateKey eAlt)
            {
                return dbSet
                    .AsNoTracking()
                    .OfType<IModelAltenateKey>()
                    .Any(c => c.Uuid == eAlt.Uuid);
            }
            else
            {
                return dbSet
                    .AsNoTracking()
                    .Any(c => c.Id.Equals(e.Id));
            }
        }

        /// <summary>
        /// Get entity by primary key.
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity Get(ID id)
        {
            TEntity found = dbSet.Find(id);
            if (found != null) dbContext.Entry(found).State = EntityState.Detached;
            return found;
        }

        /// <summary>
        /// <para>
        /// Get entity by primary key.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity GetTracking(ID id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// Get entity by alternate key.<br/>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </summary>
        /// <param name="uuid">alternate key uuid</param>
        /// <returns>found entity, otherwise null value</returns>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public virtual TEntity GetByUuid(Guid uuid)
        {
            return dbSet
                .AsNoTracking()
                .OfType<IModelAltenateKey>()
                .Where(t => t.Uuid == uuid)
                .OfType<TEntity>()
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get entity by alternate key.
        /// <para>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="uuid">alternate key uuid</param>
        /// <returns>found entity, otherwise null value</returns>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public virtual TEntity GetByUuidTracking(Guid uuid)
        {
            return dbSet
                .OfType<IModelAltenateKey>()
                .Where(t => t.Uuid == uuid)
                .OfType<TEntity>()
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the first entity in collection.
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity First()
        {
            return dbSet
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// Attempt to find the first entity in where condition.
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>first element in condition, otherwise null</returns>
        public virtual TEntity First(Expression<Func<TEntity, bool>> whereCondition)
        {
            return this.dbSet
                .AsNoTracking()
                .Where(whereCondition)
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// <para>
        /// Get the first entity in collection.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity FirstTracking()
        {
            return dbSet
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// <para>
        /// Get the first entity in where condition.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity FirstTracking(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .Where(whereCondition)
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the last entity in collection.
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity Last()
        {
            return dbSet
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the last entity in where condition.
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity Last(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .AsNoTracking()
                .Where(whereCondition)
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// <para>
        /// Get the last entity in collection.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity LastTracking()
        {
            return dbSet
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// <para>
        /// Get the last entity in where condition.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>found entity, otherwise null value</returns>
        public virtual TEntity LastTracking(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .Where(whereCondition)
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="index">item index on persistence base, from 0</param>
        /// <param name="count">entity count by page list</param>
        /// <returns>found value, otherwhise empty list.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> value is less then zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> value is less or equals zero.
        /// </exception>
        public virtual List<TEntity> PagingIndex(int index, int count)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            else if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return this.dbSet
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .Skip(index)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// List entities by paging in where condition.
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <param name="index">item index on persistence base, from 0</param>
        /// <param name="count">entity count by page list</param>
        /// <returns>found value, otherwhise empty list.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> value is less then zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> value is less or equals zero.
        /// </exception>
        public virtual List<TEntity> PagingIndex(Expression<Func<TEntity, bool>> whereCondition, int index, int count)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            else if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return this.dbSet
                .AsNoTracking()
                .Where(whereCondition)
                .OrderBy(e => e.Id)
                .Skip(index)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// <para>
        /// List entities by paging.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="index">item index on persistence base, from 0</param>
        /// <param name="count">entity count by page list</param>
        /// <returns>found value, otherwhise empty list.</returns>
        public virtual List<TEntity> PagingIndexTracking(int index, int count)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            else if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return this.dbSet
                .OrderBy(e => e.Id)
                .Skip(index)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// <para>
        /// List entities by paging in where condition.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <param name="index">item index on persistence base, from 0</param>
        /// <param name="count">entity count by page list</param>
        /// <returns>found value, otherwhise empty list.</returns>
        public virtual List<TEntity> PagingIndexTracking(Expression<Func<TEntity, bool>> whereCondition, int index, int count)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            else if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return this.dbSet
                .Where(whereCondition)
                .OrderBy(e => e.Id)
                .Skip(index)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <returns>found value, otherwhise empty list.</returns>
        public List<TEntity> Paging(int page, int limit)
        {
            limit = limit <= 0 ? IService<TEntity,ID>.REQUEST_LIST_LIMIT : limit;
            int index = Math.Max(page, 0) * limit;//to skip.
            return PagingIndex(index, limit);
        }
        
        /// <summary>
        /// <para>
        /// List entities by paging.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <returns>found value, otherwhise empty list.</returns>
        public List<TEntity> PagingTracking(int page = 0, int limit = -1)
        {
            limit = limit <= 0 ? IService<TEntity, ID>.REQUEST_LIST_LIMIT : limit;
            int index = Math.Max(page, 0) * limit;//to skip.
            return PagingIndexTracking(index, limit);
        }

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <returns>list all values possible</returns>
        public List<TEntity> List()
        {
            return PagingIndex(0, IService<TEntity, ID>.REQUEST_LIST_LIMIT);
        }

        /// <summary>
        /// <para>
        /// List all values in database
        /// (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging)
        /// in where condition.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>list all values possible</returns>
        public List<TEntity> List(Expression<Func<TEntity, bool>> whereCondition)
        {
            return PagingIndex(whereCondition, 0, IService<TEntity, ID>.REQUEST_LIST_LIMIT);
        }

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>list all values possible</returns>
        public List<TEntity> ListTracking()
        {
            return PagingIndexTracking(0, IService<TEntity, ID>.REQUEST_LIST_LIMIT);
        }

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging)
        /// in where condition.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>list all values possible</returns>
        public List<TEntity> ListTracking(Expression<Func<TEntity, bool>> whereCondition)
        {
            return PagingIndexTracking(whereCondition, 0, IService<TEntity, ID>.REQUEST_LIST_LIMIT);
        }

        /// <summary>
        /// <para>
        /// Recovery an amount of values sorted by id.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="count"> amount of data</param>
        /// <returns>list values requested sorted and limited to count</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> value is less or equals zero.
        /// </exception>
        public List<TEntity> Take(int count)
        {
            return PagingIndex(0, count);
        }

        /// <summary>
        /// <para>
        /// Recovery an amount of values sorted by id.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="count"> amount of data</param>
        /// <returns>list values requested sorted and limited to count</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> value is less or equals zero.
        /// </exception>
        public List<TEntity> TakeTracking(int count)
        {
            return PagingIndexTracking(0, count);
        }

        /// <summary>
        /// <para>
        /// Recovery a sample of values non sorted.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="count">amount of data</param>
        /// <returns>list values requested non sorted and limited to count</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> value is less or equals zero.
        /// </exception>
        public virtual List<TEntity> Sample(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            //oh! a "Take" request without "OrderBy", it's ok!
            //check de method message it is the purpose.
            return this.dbSet
                .AsNoTracking()                
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// <para>
        /// Recovery a sample of values non sorted.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="count">amount of data</param>
        /// <returns>list values requested non sorted and limited to count</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> value is less or equals zero.
        /// </exception>
        public virtual List<TEntity> SampleTracking(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            //oh! a "Take" request without "OrderBy", it's ok!
            //check de method message it is the purpose.
            return this.dbSet
                .Take(count)
                .ToList();
        }
        #endregion

        #region [U]pdate
        /// <summary>
        /// Update entity on persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <returns>updated target entity</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid (when implementing <see cref="IModelAltenateKey"/>).</exception>
        /// <exception cref="DbUpdateException">throws when is not possible update value, value does not exists, for example.</exception>
        public TEntity Update(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            else if (Objects.Compare(entity.Id, default))
            {
                if(entity is IModelAltenateKey altKey)
                {
                    if(Objects.Compare(altKey.Uuid, default))
                    {
                        throw new InvalidOperationException(
                            $"Entity \"{typeof(TEntity).Name}\" is untrackable, " +
                            "thus can not be updated! Because it does not contains an Id or Uuid.");
                    }
                } 
                else
                {
                    throw new InvalidOperationException(
                      $"Entity \"{typeof(TEntity).Name}\" is untrackable, " +
                      "thus can not be updated! Because it does not contains an Id.");
                }
            }

            OnBeforeUpdateCallback(entity);
            RequireValidate(entity);
            //check contains in tracking local dbSet.
            TEntity curr = dbSet.Local.FirstOrDefault(entity.EqualsAnyId);

            if (curr != null)
            {
                dbContext.Entry(OnValidateEntryBeforeUpdateCallback(curr, entity))
                    .CurrentValues
                    .SetValues(entity);
            }
            else if(Objects.Compare(entity.Id, default))
            {
                IModelAltenateKey altKey = entity as IModelAltenateKey ??
                    throw new InvalidCastException($"Entity \"{typeof(TEntity).Name}\" " +
                        $"does not implements {typeof(IModelAltenateKey).Name}!");

                curr = dbSet
                   .OfType<IModelAltenateKey>()
                   .Where(t => t.Uuid == altKey.Uuid)
                   .OfType<TEntity>()
                   .OrderBy(t => t.Id)
                   .Take(1)
                   .FirstOrDefault();

                if (curr == null)
                {
                    throw new DbUpdateException($"Entity \"{typeof(TEntity).Name}\" " +
                        $"with Uuid \"{altKey.Uuid}\" does not exists on database!");
                }

                try
                {
                    entity.Id = curr.Id;
                    dbContext.Entry(OnValidateEntryBeforeUpdateCallback(curr, entity))
                        .CurrentValues
                        .SetValues(entity);
                }
                catch
                {
                    dbContext.Entry(curr).State = EntityState.Detached;
                    throw;
                }
            }
            else
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            try
            {
                dbContext.SaveChanges();

                if(curr != null)
                {
                    dbContext.Entry(curr).State = EntityState.Detached;
                    dbContext.Entry(entity).CurrentValues.SetValues(curr);
                }

                OnUpdatedCallback(entity);
            }
            finally
            {
                dbContext.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }
        #endregion

        #region [D]elete
        internal IEnumerable<TEntity> AttachRangeNonExists(IEnumerable<TEntity> entities)
        {
            foreach(TEntity e in entities)
            {
                TEntity curr = dbSet.Local.FirstOrDefault(e.EqualsAnyId);

                if(curr != null)
                {
                    yield return curr;
                }
                else if(Objects.Compare(e.Id, default))
                {
                    if(e is IModelAltenateKey altKey && GetByUuid(altKey.Uuid) is TEntity found)
                    {
                        yield return found;
                    }
                    else
                    {
                        #if DEBUG
                        throw new InvalidOperationException($"Entity \"{typeof(TEntity).Name}\" is untrackable, " +
                            $"thus can not be attached to delete!");
                        #else
                        continue;
                        #endif
                    }
                }
                else
                {
                    yield return e;
                }
            }
        }

        internal IEnumerable<TEntity> AttachRangeNonExists(IEnumerable<Guid> uuids)
        {
            RequireEntityImplementIModelAlternateKey();
            return AttachRangeNonExists(uuids.Select(uuid => 
            {
                TEntity t = new TEntity { };
                IModelAltenateKey altKey = (IModelAltenateKey) t;
                altKey.Uuid = uuid;
                return t;
            }));
        }

        internal virtual int DeleteLocal(IEnumerable<Guid> uuids)
        {
            var entity  = AttachRangeNonExists(uuids).ToList();
            OnBeforeDeleteCallback(entity);
            dbSet.RemoveRange(entity);
            int count   = dbContext.SaveChanges();
            if(count > 0) OnDeletedCallback(entity);
            return Math.Min(count, entity.Count);
        }

        /// <summary>
        /// Attempt to delete values by id.<br/>
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>value removed</returns>
        public bool Delete(ID id)
        {
            return Delete(new TEntity { Id = id });
        }

        /// <summary>
        /// Attempt to delete values by uuid.<br/>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </summary>
        /// <param name="uuids">uuids target</param>
        /// <returns>amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public int DeleteByUuid(IEnumerable<Guid> uuids)
        {
            return DeleteLocal(uuids);
        }

        /// <summary>
        /// Attempt to delete values by uuid.<br/>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </summary>
        /// <param name="args">uuids target</param>
        /// <returns>amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public int DeleteByUuid(params Guid[] args)
        {
            return DeleteLocal(args);
        }

        /// <summary>
        /// Attempt to delete values by uuid.<br/>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public bool DeleteByUuid(Guid uuid)
        {
            return DeleteLocal(new Guid[] { uuid }) == 1;
        }

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public virtual bool Delete(IEnumerable<TEntity> entity)
        {
            var att = AttachRangeNonExists(entity);
            OnBeforeDeleteCallback(att);
            dbSet.RemoveRange(att);
            var res = dbContext.SaveChanges() >= entity.Count();
            if (res) OnDeletedCallback(att);
            return res;
        }

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public virtual bool Delete(params TEntity[] entity)
        {
            var att = AttachRangeNonExists(entity);
            OnBeforeDeleteCallback(att);
            dbSet.RemoveRange(att);
            var res = dbContext.SaveChanges() >= entity.Length;
            if (res) OnDeletedCallback(att);
            return res;
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Callback firing before insert entity.
        /// </summary>
        /// <param name="entity">new entity</param>
        protected virtual void OnBeforeInsertCallback(TEntity entity) { }

        /// <summary>
        /// Callback firing before update entity.
        /// </summary>
        /// <param name="entity">entity updated</param>
        protected virtual void OnBeforeUpdateCallback(TEntity entity) { }

        /// <summary>
        /// Callback firing before delete entities.
        /// </summary>
        /// <param name="entities">entities deleted</param>
        protected virtual void OnBeforeDeleteCallback(IEnumerable<TEntity> entities) { }

        /// <summary>
        /// Callback firing after insert entity.
        /// </summary>
        /// <param name="entity">new entity</param>
        protected virtual void OnInsertedCallback(TEntity entity) { }

        /// <summary>
        /// Callback firing after update entity.
        /// </summary>
        /// <param name="entity">entity updated</param>
        protected virtual void OnUpdatedCallback(TEntity entity) { }

        /// <summary>
        /// Callback firing after delete entities.
        /// </summary>
        /// <param name="entities">entities deleted</param>
        protected virtual void OnDeletedCallback(IEnumerable<TEntity> entities) { }

        /// <summary>
        /// Callback firing before update to validate current entity and candidate update entity.
        /// </summary>
        /// <param name="currentEntityInDatabase">current entity values in database</param>
        /// <param name="candidateEntityToUpdate">candidate value to update entity</param>
        /// <returns></returns>
        protected virtual TEntity OnValidateEntryBeforeUpdateCallback(
            TEntity currentEntityInDatabase,
            TEntity candidateEntityToUpdate)
        {
            return currentEntityInDatabase;
        }
        #endregion

    }
}
