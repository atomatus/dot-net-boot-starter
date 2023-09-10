using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// CRUD Operations for DBContext/DBSet.
    /// </summary>
    /// <typeparam name="TContext">context type</typeparam>
    /// <typeparam name="TEntity">entity type</typeparam>
    public abstract partial class Crud<TContext, TEntity> : ICrud<TEntity>
        where TContext : ContextBase
        where TEntity : class, IModel, new()
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
        internal Crud([NotNull] TContext context, [NotNull] DbSet<TEntity> dbSet)
        {
            this.dbContext = context ?? throw new ArgumentNullException(nameof(context));
            this.dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        }

        /// <summary>
        /// Constructor for dbcontext and
        /// autodiscovery dbset.
        /// </summary>
        /// <param name="context">target dbcontext.</param>
        internal Crud([NotNull] TContext context) : this(context, context?.GetOrSet<TEntity>()) { }

        #region [C]reate
        /// <summary>
        /// Insert a new value to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <returns>entity within ids</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        public virtual TEntity Save(TEntity entity)
        {
            OnBeforeInsertCallback(entity);
            RequireValidateModel(entity);
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
        internal static void RequireEntityImplementIModelAlternateKey()
        {
            if (!typeof(IModelAltenateKey).IsAssignableFrom(typeof(TEntity)))
            {
                throw new InvalidCastException($"Is not possible manipulate, find or delete " +
                    $"value of Entity \"{typeof(TEntity).Name}\" using Uuid, because " +
                    $"this class does not implements {typeof(IModelAltenateKey).Name} contract!");
            }
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
        /// Check if current entity exists on persistence base.
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
                return this.ExistsById(e);
            }
        }

        /// <summary>
        /// Check if current entity exists by ID.
        /// </summary>
        /// <param name="e">target entity</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">current implementation is not valid to find by id</exception>
        internal virtual bool ExistsById(TEntity e)
        {
            throw new InvalidOperationException("Target entity does not contains IModelAlternateKey, " +
                "implement it or use CrudId instead Crud to find by Id!");
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
                .OfType<IAudit>()
                .OrderByDescending(t => t.Created)
                .OfType<TEntity>()
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
                 .OfType<IAudit>()
                 .OrderByDescending(t => t.Created)
                 .OfType<TEntity>()
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
                .OfType<IAudit>()
                .OrderBy(e => e.Created)
                .OfType<TEntity>()
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
                .OfType<IAudit>()
                .OrderBy(e => e.Created)
                .OfType<TEntity>()
                .Skip(index)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>).</param>
        /// <returns>found value, otherwhise empty list.</returns>
        public List<TEntity> Paging(int page, int limit)
        {
            limit = limit <= 0 ? ICrud<TEntity>.REQUEST_LIST_LIMIT : limit;
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
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>).</param>
        /// <returns>found value, otherwhise empty list.</returns>
        public List<TEntity> PagingTracking(int page = 0, int limit = -1)
        {
            limit = limit <= 0 ? ICrud<TEntity>.REQUEST_LIST_LIMIT : limit;
            int index = Math.Max(page, 0) * limit;//to skip.
            return PagingIndexTracking(index, limit);
        }

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
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
            return PagingIndex(0, ICrud<TEntity>.REQUEST_LIST_LIMIT);
        }

        /// <summary>
        /// <para>
        /// List all values in database
        /// (limited to max request <see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>, when more that it, use paging)
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
            return PagingIndex(whereCondition, 0, ICrud<TEntity>.REQUEST_LIST_LIMIT);
        }

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
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
            return PagingIndexTracking(0, ICrud<TEntity>.REQUEST_LIST_LIMIT);
        }

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>, when more that it, use paging)
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
            return PagingIndexTracking(whereCondition, 0, ICrud<TEntity>.REQUEST_LIST_LIMIT);
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
        public virtual TEntity Update(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            else if (entity is IModelAltenateKey altKey)
            {
                if (Objects.Compare(altKey.Uuid, default))
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

            OnBeforeUpdateCallback(entity);
            RequireValidateModel(entity);
            //check contains in tracking local dbSet.
            TEntity curr = dbSet.Local.FirstOrDefault(e => e is IModelEquatable em ?
                em.EqualsAnyId(entity) : e.Equals(entity));

            if (curr != null)
            {
                dbContext.Entry(OnValidateEntryBeforeUpdateCallback(curr, entity))
                    .CurrentValues
                    .SetValues(entity);
            }
            else
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            try
            {
                dbContext.SaveChanges();

                if (curr != null)
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
        internal virtual IEnumerable<TEntity> AttachRangeNonExists(IEnumerable<TEntity> entities)
        {
            var values = dbSet.Local.AsEnumerable();

            foreach (TEntity e in entities)
            {
                TEntity curr = values
                    .FirstOrDefault(c => c is IModelEquatable me ?
                        me.EqualsAnyId(e) : c.Equals(e));

                if (curr != null)
                {
                    yield return curr;
                }
                if (e is IModelAltenateKey altKey && GetByUuid(altKey.Uuid) is TEntity found)
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
        }

        internal IEnumerable<TEntity> AttachRangeNonExists(IEnumerable<Guid> uuids)
        {
            RequireEntityImplementIModelAlternateKey();
            return AttachRangeNonExists(uuids.Select(uuid =>
            {
                TEntity t = new TEntity();
                IModelAltenateKey altKey = (IModelAltenateKey)t;
                altKey.Uuid = uuid;
                return t;
            }));
        }

        internal virtual int DeleteLocal(IEnumerable<Guid> uuids)
        {
            int count = dbSet.OfType<IModelAltenateKey>()
                .Where(e => uuids.Any(uuid => uuid == e.Uuid))
                .ExecuteDelete();
            return Math.Min(count, uuids.Count());
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
