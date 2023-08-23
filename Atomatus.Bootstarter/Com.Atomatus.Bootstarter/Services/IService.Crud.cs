using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Entity service CRUD for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public interface IServiceCrud<TEntity> : IService<TEntity>
        where TEntity : IModel
    {
        #region [C]reate
        /// <summary>
        /// Insert a new valeu to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <returns>entity within ids</returns>
        TEntity Save(TEntity entity);
        #endregion

        #region [R]ead
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
        bool ExistsByUuid(Guid uuid);

        /// <summary>
        /// Check whether current entity exists on persistence base.
        /// </summary>
        /// <param name="e">target entity</param>
        /// <returns>true, value exists, otherwhise false</returns>
        bool Exists(TEntity e);

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
        /// Obs.: This request is Tracking disabled (<see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TEntity}(System.Linq.IQueryable{TEntity})"/>).
        /// </para>
        /// </summary>
        /// <param name="uuid">alternate key uuid</param>
        /// <returns>found entity, otherwise null value</returns>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        TEntity GetByUuid(Guid uuid);

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
        TEntity GetByUuidTracking(Guid uuid);

        /// <summary>
        /// Get the first entity in collection.
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        TEntity First();

        /// <summary>
        /// Attempt to find the first entity in where condition.
        /// </summary>
        /// <param name="whereCondition">where filter condition</param>
        /// <returns>first element in condition, otherwise null</returns>
        TEntity First(Expression<Func<TEntity, bool>> whereCondition);

        /// <summary>
        /// <para>
        /// Get the first entity in collection.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        TEntity FirstTracking();

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
        TEntity FirstTracking(Expression<Func<TEntity, bool>> whereCondition);

        /// <summary>
        /// Get the last entity in collection.
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        TEntity Last();

        /// <summary>
        /// Get the last entity in where condition.
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        TEntity Last(Expression<Func<TEntity, bool>> whereCondition);

        /// <summary>
        /// <para>
        /// Get the last entity in collection.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>found entity, otherwise null value</returns>
        TEntity LastTracking();

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
        TEntity LastTracking(Expression<Func<TEntity, bool>> whereCondition);

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="index">item index on persistence base, from 0</param>
        /// <param name="count">entity count by page list</param>
        /// <returns>found value, otherwhise empty list.</returns>
        List<TEntity> PagingIndex(int index, int count);

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
        List<TEntity> PagingIndex(Expression<Func<TEntity, bool>> whereCondition, int index, int count);

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
        List<TEntity> PagingIndexTracking(int index, int count);

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
        List<TEntity> PagingIndexTracking(Expression<Func<TEntity, bool>> whereCondition, int index, int count);

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <returns>found value, otherwhise empty list.</returns>
        List<TEntity> Paging(int page = 0, int limit = -1);

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
        List<TEntity> PagingTracking(int page = 0, int limit = -1);

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
        List<TEntity> List();

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
        List<TEntity> List(Expression<Func<TEntity, bool>> whereCondition);

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
        List<TEntity> ListTracking();

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
        public List<TEntity> ListTracking(Expression<Func<TEntity, bool>> whereCondition);

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
        List<TEntity> Take(int count);

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
        List<TEntity> TakeTracking(int count);

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
        List<TEntity> Sample(int count);

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
        List<TEntity> SampleTracking(int count);
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
        TEntity Update(TEntity entity);
        #endregion

        #region [D]elete
        /// <summary>
        /// Attempt to delete values by uuid.<br/>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <returns>amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        int DeleteByUuid(IEnumerable<Guid> uuid);

        /// <summary>
        /// Attempt to delete values by uuid.<br/>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <returns>amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        int DeleteByUuid(params Guid[] uuid);

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
        bool DeleteByUuid(Guid uuid);

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        bool Delete(IEnumerable<TEntity> entity);

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        bool Delete(params TEntity[] entity);
        #endregion
    }

    /// <summary>
    /// Entity service CRUD for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IServiceCrud<TEntity, ID> : IServiceCrud<TEntity>, IService<TEntity, ID>
        where TEntity : IModel<ID>
    {
        #region [R]ead      
        /// <summary>
        /// Check whether current ID exists on persistence base.<br/>
        /// </summary>
        /// <param name="id">primary key</param>
        /// <returns>true, value exists, otherwhise false</returns>
        bool Exists(ID id);

        /// <summary>
        /// Check if where condition makes matches with some data.
        /// </summary>
        /// <param name="whereCondition">where condition</param>
        /// <returns>true if any elements in the source sequence pass the test in the specified predicate; otherwise, false.</returns>
        bool Exists(Expression<Func<TEntity, bool>> whereCondition);

        /// <summary>
        /// Get entity by primary key.
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>found entity, otherwise null value</returns>
        TEntity Get(ID id);

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
        TEntity GetTracking(ID id);
        #endregion

        #region [D]elete
        /// <summary>
        /// Attempt to delete values by id.<br/>
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>value removed</returns>
        bool Delete(ID id);
        #endregion
    }
}
