using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter
{
    public abstract partial class Crud<TContext, TEntity> : ICrudExpression<TEntity>
    {
        #region [R]ead
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
        /// Attempt to find the first entity in where condition.
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>first element in condition, otherwise null</returns>
        public virtual TEntity First(Expression<Func<TEntity, bool>> whereCondition)
        {
            return this.dbSet
                .AsNoTracking()
                .Where(whereCondition)
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
                .OfType<IAudit>()
                .OrderByDescending(t => t.Created)
                .OfType<TEntity>()
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
                 .OfType<IAudit>()
                 .OrderByDescending(t => t.Created)
                 .OfType<TEntity>()
                 .Take(1)
                 .FirstOrDefault();
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
                .OfType<IAudit>()
                .OrderBy(e => e.Created)
                .OfType<TEntity>()
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
                .OfType<IAudit>()
                .OrderBy(e => e.Created)
                .OfType<TEntity>()
                .Skip(index)
                .Take(count)
                .ToList();
        }
        #endregion
    }
}
