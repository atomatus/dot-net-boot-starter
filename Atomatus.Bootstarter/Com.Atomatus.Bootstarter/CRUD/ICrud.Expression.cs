using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// CRUD Contract class for expression callback function usage in requests.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
	public interface ICrudExpression<TEntity> : ICrud<TEntity> where TEntity : IModel
    {
        #region [R]ead
        /// <summary>
        /// Attempt to find the first entity in where condition.
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>first element in condition, otherwise null</returns>
        /// <exception cref="ArgumentNullException">Throws when whereCondition is null</exception>
        TEntity First(Expression<Func<TEntity, bool>> whereCondition);

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
        /// <exception cref="ArgumentNullException">Throws when whereCondition is null</exception>
        TEntity FirstTracking(Expression<Func<TEntity, bool>> whereCondition);

        /// <summary>
        /// Get the last entity in where condition.
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        /// <exception cref="ArgumentNullException">Throws when whereCondition is null</exception>
        TEntity Last(Expression<Func<TEntity, bool>> whereCondition);

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
        /// <exception cref="ArgumentNullException">Throws when whereCondition is null</exception>
        TEntity LastTracking(Expression<Func<TEntity, bool>> whereCondition);

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
        /// <exception cref="ArgumentNullException">Throws when whereCondition is null</exception>
        List<TEntity> PagingIndex(Expression<Func<TEntity, bool>> whereCondition, int index, int count);

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
        /// <exception cref="ArgumentNullException">Throws when whereCondition is null</exception>
        List<TEntity> PagingIndexTracking(Expression<Func<TEntity, bool>> whereCondition, int index, int count);

        /// <summary>
        /// <para>
        /// List all values in database
        /// (limited to max request <see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>, when more that it, use paging)
        /// in where condition.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="ICrud{TEntity}.Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>list all values possible</returns>
        /// <exception cref="ArgumentNullException">Throws when whereCondition is null</exception>
        List<TEntity> List(Expression<Func<TEntity, bool>> whereCondition);

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>, when more that it, use paging)
        /// in where condition.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="ICrud{TEntity}.Paging(int, int)"/>.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>list all values possible</returns>
        /// <exception cref="ArgumentNullException">Throws when whereCondition is null</exception>
        List<TEntity> ListTracking(Expression<Func<TEntity, bool>> whereCondition);

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">filter predicate callback</param>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <exception cref="ArgumentNullException">Throws when predicate is null</exception>
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        #endregion
    }
}
