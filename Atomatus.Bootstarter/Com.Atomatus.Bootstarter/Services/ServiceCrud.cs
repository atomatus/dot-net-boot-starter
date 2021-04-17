using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Abstract Service Crud.
    /// </summary>
    /// <typeparam name="TContext">target DbContext</typeparam>
    /// <typeparam name="TEntity">target entity</typeparam>
    /// <typeparam name="ID">target entity id</typeparam>
    public abstract partial class ServiceCrud<TContext, TEntity, ID> : IServiceCrud<TEntity, ID>
        where TEntity : ModelBase<ID>, new()
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
            this.dbContext  = context ?? throw new ArgumentNullException(nameof(context));
            this.dbSet      = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        }

        /// <summary>
        /// Constructor for dbcontext and
        /// autodiscovery dbset.
        /// </summary>
        /// <param name="context">target dbcontext.</param>
        public ServiceCrud([NotNull] TContext context) : this(context, context?.GetOrSet<TEntity>()) { }

        #region [C]reate
        /// <summary>
        /// Insert a new valeu to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <returns>entity within ids</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        public TEntity Insert(TEntity entity)
        {
            dbSet.Add(entity ?? throw new ArgumentNullException(nameof(entity)));
            dbContext.SaveChanges();
            OnInsertedCallback(entity);
            return entity;
        }
        #endregion

        #region [R]ead        

        /// <summary>
        /// Check whether current uuid exists on persistence base.
        /// </summary>
        /// <param name="uuid">alternate key uuid</param>
        /// <returns>true, value exists, otherwhise false</returns>
        public bool Exists(Guid uuid)
        {
            return dbSet
                .AsNoTracking()
                .Any(e => e.Uuid == uuid);
        }

        /// <summary>
        /// Check whether current entity exists on persistence base.
        /// </summary>
        /// <param name="e">target entity</param>
        /// <returns>true, value exists, otherwhise false</returns>
        public bool Exists(TEntity e)
        {
            return dbSet
                .AsNoTracking()
                .Any(c => c.Uuid == e.Uuid);
        }

        /// <summary>
        /// Get entity by primary key.
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>found entity, otherwise null value</returns>
        public TEntity Get(ID id)
        {
            TEntity found = dbSet.Find(id);
            if (found != null) dbContext.Entry(found).State = EntityState.Detached;
            return found;
        }

        /// <summary>
        /// Get entity by alternate key.
        /// </summary>
        /// <param name="uuid">target alternate key</param>
        /// <returns>found entity, otherwise null value</returns>
        public TEntity GetByUuid(Guid uuid)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Uuid == uuid)
                .OrderBy(e => e.Id)
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
        public List<TEntity> PagingIndex(int index, int count)
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
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <returns>found value, otherwhise empty list.</returns>
        public List<TEntity> Paging(int page, int limit)
        {
            page = page < 0 ? 0 : page;
            limit = limit <= 0 ? IService<TEntity,ID>.REQUEST_LIST_LIMIT : limit;
            int index = page * limit;//to skip.
            return PagingIndex(index, limit);
        }

        /// <summary>
        /// List all values in database (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
        /// </summary>
        /// <returns>list all values possible</returns>
        public List<TEntity> List()
        {
            return PagingIndex(0, IService<TEntity, ID>.REQUEST_LIST_LIMIT);
        }
        #endregion

        #region [U]pdate
        /// <summary>
        /// Update entity on persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <returns>updated target entity</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="DbUpdateException">throws when is not possible update value, value does not exists, for example.</exception>
        public TEntity Update(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            else if (Objects.Compare(entity.Id, default) && Objects.Compare(entity.Uuid, default))
            {
                throw new InvalidOperationException($"Entity \"{typeof(TEntity).Name}\" is untrackable, " +
                    "thus can not be updated! " +
                    "Because it does not contains an Id or Uuid.");
            }

            TEntity curr = dbSet.Local.FirstOrDefault(entity.EqualsAnyId);

            if (curr != null)
            {
                dbContext.Entry(curr)
                    .CurrentValues
                    .SetValues(entity);
            }
            else if(Objects.Compare(entity.Id, default))
            {
                curr = dbSet
                   .Where(t => t.Uuid == entity.Uuid)
                   .OrderBy(e => e.Id)
                   .Take(1)
                   .FirstOrDefault();

                if (curr == null)
                {
                    throw new DbUpdateException($"Entity \"{typeof(TEntity).Name}\" with Uuid \"{entity.Uuid}\" does not exist on database!");
                }

                entity.Id = curr.Id;
                dbContext.Entry(curr)
                    .CurrentValues
                    .SetValues(entity);
            }
            else
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            dbContext.SaveChanges();
            OnUpdatedCallback(entity);
            return entity;
        }
        #endregion

        #region [D]elete
        private IEnumerable<TEntity> AttachRangeNonExists(IEnumerable<TEntity> entity)
        {
            foreach(TEntity e in entity)
            {
                TEntity curr = dbSet.Local.FirstOrDefault(e.EqualsAnyId);                
                if(curr != null || (Objects.Compare(e.Id, default) && (curr = GetByUuid(e.Uuid)) != null))
                {
                    yield return curr;
                }
                else if(Objects.Compare(e.Id, default))
                {
                    #if DEBUG
                    throw new InvalidOperationException($"Entity \"{typeof(TEntity).Name}\" is untrackable, " +
                        $"thus can not be attached to delete!");
                    #else
                    continue;
                    #endif
                }
                else
                {
                    yield return e;
                }
            }
        }

        private int DeleteLocal(IEnumerable<Guid> uuids)
        {
            var aux     = uuids.Select(i => new TEntity() { Uuid = i });
            var entity  = AttachRangeNonExists(aux).ToList();
            dbSet.RemoveRange(entity);
            int count   = dbContext.SaveChanges();
            if(count > 0) OnDeletedCallback(entity);
            return Math.Min(count, entity.Count);
        }

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuids">uuids target</param>
        /// <returns>amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public int Delete(IEnumerable<Guid> uuids)
        {
            return DeleteLocal(uuids);
        }

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="args">uuids target</param>
        /// <returns>amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public int Delete(params Guid[] args)
        {
            return DeleteLocal(args);
        }

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public bool Delete(Guid uuid)
        {
            return DeleteLocal(new Guid[] { uuid }) == 1;
        }

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public bool Delete(IEnumerable<TEntity> entity)
        {
            var att = AttachRangeNonExists(entity);
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
        public bool Delete(params TEntity[] entity)
        {
            var att = AttachRangeNonExists(entity);
            dbSet.RemoveRange(att);
            var res = dbContext.SaveChanges() >= entity.Length;
            if (res) OnDeletedCallback(att);
            return res;
        }
        #endregion

        #region Callbacks
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
        #endregion

    }
}
