using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter
{
    public abstract partial class CrudId<TContext, TEntity, ID>
        : Crud<TContext, TEntity>, ICrud<TEntity, ID> 
        where TContext : ContextBase
        where TEntity : class, IModel<ID>, new()
    {
        /// <inheritdoc/>
        internal CrudId([NotNull] TContext context, [NotNull] DbSet<TEntity> dbSet)
            : base(context, dbSet) { }

        /// <inheritdoc/>
        internal CrudId([NotNull] TContext context)
            : base(context) { }

        #region [R]ead
        /// <inheritdoc/>
        public bool Exists(ID id)
        {
            return dbSet
                .AsNoTracking()
                .Any(e => e.Id.Equals(id));
        }

        /// <inheritdoc/>
        internal override bool ExistsById(TEntity e)
        {
            return this.Exists(e.Id);
        }

        /// <inheritdoc/>
        public virtual TEntity Get(ID id)
        {
            TEntity found = dbSet.Find(id);
            if (found != null) dbContext.Entry(found).State = EntityState.Detached;
            return found;
        }

        /// <inheritdoc/>
        public virtual TEntity GetTracking(ID id)
        {
            return dbSet.Find(id);
        }

        /// <inheritdoc/>
        public override TEntity GetByUuid(Guid uuid)
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

        /// <inheritdoc/>
        public override TEntity GetByUuidTracking(Guid uuid)
        {
            return dbSet
                .OfType<IModelAltenateKey>()
                .Where(t => t.Uuid == uuid)
                .OfType<TEntity>()
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity First()
        {
            return dbSet
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity FirstTracking()
        {
            return dbSet
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity Last()
        {
            return dbSet
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity LastTracking()
        {
            return dbSet
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override List<TEntity> PagingIndex(int index, int count)
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

        /// <inheritdoc/>
        public override List<TEntity> PagingIndexTracking(int index, int count)
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
        #endregion

        #region [U]pdate
        /// <inheritdoc/>
        public override TEntity Update(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            else if (Objects.Compare(entity.Id, default))
            {
                if (entity is IModelAltenateKey altKey)
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
            }

            OnBeforeUpdateCallback(entity);
            RequireValidateModel(entity);
            //check contains in tracking local dbSet.
            TEntity curr = dbSet.Local.FirstOrDefault(entity.EqualsAnyId);

            if (curr != null)
            {
                dbContext.Entry(OnValidateEntryBeforeUpdateCallback(curr, entity))
                    .CurrentValues
                    .SetValues(entity);
            }
            else if (Objects.Compare(entity.Id, default))
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
        internal override IEnumerable<TEntity> AttachRangeNonExists(IEnumerable<TEntity> entities)
        {
            foreach (TEntity e in entities)
            {
                TEntity curr = dbSet.Local.FirstOrDefault(e.EqualsAnyId);

                if (curr != null)
                {
                    yield return curr;
                }
                else if (Objects.Compare(e.Id, default))
                {
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
                else
                {
                    yield return e;
                }
            }
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
        #endregion
    }
}
