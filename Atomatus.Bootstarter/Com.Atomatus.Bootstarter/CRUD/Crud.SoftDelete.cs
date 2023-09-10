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
    /// Soft Delete Abstract CRUD.
    ///<para>
    /// When model is ISoftDeleteModel, instead delete it, setup a value in Deleted property
    /// and this data is not more accessible by common methods in Service. Buts keep it
    /// in database for historic track purporse.
    ///</para>
    /// </summary>
    /// <typeparam name="TContext">target DbContext</typeparam>
    /// <typeparam name="TEntity">target entity</typeparam>
    public abstract class CrudSoftDelete<TContext, TEntity> : Crud<TContext, TEntity>
        where TEntity : class, ISoftDeleteModel, new()
        where TContext : ContextBase
    {
        /// <inheritdoc/>
        internal CrudSoftDelete([NotNull] TContext context, [NotNull] DbSet<TEntity> dbSet)
            : base(context, dbSet) { }

        /// <inheritdoc/>
        internal CrudSoftDelete([NotNull] TContext context)
            : base(context) { }

        #region [R]ead
        /// <inheritdoc/>
        public override TEntity GetByUuid(Guid uuid)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Deleted == null && t.Uuid == uuid)
                .OfType<TEntity>()
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity GetByUuidTracking(Guid uuid)
        {
            return dbSet
                .Where(t => t.Deleted == null && t.Uuid == uuid)
                .OfType<TEntity>()
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity First()
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Deleted == null)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity First(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Deleted == null)
                .Where(whereCondition)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity FirstTracking()
        {
            return dbSet
                .Where(t => t.Deleted == null)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity FirstTracking(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .Where(t => t.Deleted == null)
                .Where(whereCondition)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity Last()
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Deleted == null)
                .OfType<IAudit>()
                .OrderByDescending(t => t.Created)
                .OfType<TEntity>()
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity Last(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Deleted == null)
                .Where(whereCondition)
                .OfType<IAudit>()
                .OrderByDescending(t => t.Created)
                .OfType<TEntity>()
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity LastTracking()
        {
            return dbSet
                .Where(t => t.Deleted == null)
                .OfType<IAudit>()
                .OrderByDescending(t => t.Created)
                .OfType<TEntity>()
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity LastTracking(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .Where(t => t.Deleted == null)
                .Where(whereCondition)
                .OfType<IAudit>()
                .OrderByDescending(t => t.Created)
                .OfType<TEntity>()
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
                .Where(t => t.Deleted == null)
                .OfType<IAudit>()
                .OrderBy(e => e.Created)
                .OfType<TEntity>()
                .Skip(index)
                .Take(count)
                .ToList();
        }

        /// <inheritdoc/>
        public override List<TEntity> PagingIndex(Expression<Func<TEntity, bool>> whereCondition, int index, int count)
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
                .Where(t => t.Deleted == null)
                .Where(whereCondition)
                .OfType<IAudit>()
                .OrderBy(e => e.Created)
                .OfType<TEntity>()
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
                .Where(t => t.Deleted == null)
                .OfType<IAudit>()
                .OrderBy(e => e.Created)
                .OfType<TEntity>()
                .Skip(index)
                .Take(count)
                .ToList();
        }

        /// <inheritdoc/>
        public override List<TEntity> PagingIndexTracking(Expression<Func<TEntity, bool>> whereCondition, int index, int count)
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
                .Where(t => t.Deleted == null)
                .Where(whereCondition)
                .OfType<IAudit>()
                .OrderBy(e => e.Created)
                .OfType<TEntity>()
                .Skip(index)
                .Take(count)
                .ToList();
        }

        /// <inheritdoc/>
        public override List<TEntity> Sample(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            //oh! a "Take" request without "OrderBy", it's ok!
            //check de method message it is the purpose.
            return this.dbSet
                .Where(t => t.Deleted == null)
                .AsNoTracking()
                .Take(count)
                .ToList();
        }

        /// <inheritdoc/>
        public override List<TEntity> SampleTracking(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            //oh! a "Take" request without "OrderBy", it's ok!
            //check de method message it is the purpose.
            return this.dbSet
                .Where(t => t.Deleted == null)
                .Take(count)
                .ToList();
        }
        #endregion

        #region [D]elete
        /// <inheritdoc/>
        internal override int DeleteLocal(IEnumerable<Guid> uuids)
        {
            var entity = AttachRangeNonExists(uuids).ToList();
            OnBeforeDeleteCallback(entity);
            int count = dbSet.Where(e => uuids.Any(uuid => uuid == e.Uuid))
                .ExecuteUpdate(setter =>
                    setter.SetProperty(e => e.Deleted, DateTime.Now));
            if (count > 0) OnDeletedCallback(entity);
            return Math.Min(count, entity.Count);
        }

        /// <inheritdoc/>
        public override bool Delete(IEnumerable<TEntity> entities)
        {
            var att = AttachRangeNonExists(entities);
            OnBeforeDeleteCallback(att);
            int count = dbSet.Where(e => entities.Any(o => o.Uuid == e.Uuid))
                .ExecuteUpdate(setter =>
                    setter.SetProperty(e => e.Deleted, DateTime.Now));
            var res = count > 0;
            if (res) OnDeletedCallback(att);
            return res;
        }

        /// <inheritdoc/>
        public override bool Delete(params TEntity[] entities)
        {
            var att = AttachRangeNonExists(entities);
            OnBeforeDeleteCallback(att);
            int count = dbSet.Where(e => entities.Any(o => o.Uuid == e.Uuid))
                .ExecuteUpdate(setter =>
                    setter.SetProperty(e => e.Deleted, DateTime.Now));
            var res = count > 0;
            if (res) OnDeletedCallback(att);
            return res;
        }
        #endregion

        #region Callbacks
        /// <inheritdoc/>
        protected override void OnBeforeInsertCallback(TEntity entity)
        {
            entity.Deleted = null;
            base.OnBeforeInsertCallback(entity);
        }

        /// <inheritdoc/>
        protected override TEntity OnValidateEntryBeforeUpdateCallback(TEntity currentEntityInDatabase, TEntity candidateEntityToUpdate)
        {
            if (currentEntityInDatabase.Deleted != null)
            {
                throw new DbUpdateException($"Entity \"{typeof(TEntity).Name}\" " +
                        $"with Uuid \"{currentEntityInDatabase.Uuid}\" was deleted and can not be updated!");
            }

            return base.OnValidateEntryBeforeUpdateCallback(currentEntityInDatabase, candidateEntityToUpdate);
        }
        #endregion
    }
}
