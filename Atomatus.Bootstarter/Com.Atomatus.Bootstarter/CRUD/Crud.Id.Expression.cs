using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter
{
    public abstract partial class CrudId<TContext, TEntity, ID> : ICrudExpression<TEntity, ID>
    {
        #region [R]ead
        /// <inheritdoc/>
        public override TEntity First(Expression<Func<TEntity, bool>> whereCondition)
        {
            return this.dbSet
                .AsNoTracking()
                .Where(whereCondition)
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity FirstTracking(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .Where(whereCondition)
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity Last(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .AsNoTracking()
                .Where(whereCondition)
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public override TEntity LastTracking(Expression<Func<TEntity, bool>> whereCondition)
        {
            return dbSet
                .Where(whereCondition)
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefault();
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
                .Where(whereCondition)
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
                .Where(whereCondition)
                .OrderBy(e => e.Id)
                .Skip(index)
                .Take(count)
                .ToList();
        }
        #endregion
    }
}

