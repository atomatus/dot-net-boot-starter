using Com.Atomatus.Bootstarter.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Entity service CRUD Async for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public interface IServiceCrudAsync<TEntity> : IService<TEntity> 
        where TEntity : IModel
    {
        #region [C]reate
        /// <summary>
        /// Insert a new valeu to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with entity</returns>
        Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);
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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true value exists, otherwhise false</returns>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        Task<bool> ExistsByUuidAsync(Guid uuid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check whether current entity exists on persistence base.
        /// </summary>
        /// <param name="e">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true value exists, otherwhise false</returns>
        Task<bool> ExistsAsync(TEntity e, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get entity by alternate key.<br/>
        /// <i>
        /// Obs.: <typeparamref name="TEntity"/> must contains 
        /// <see cref="IModelAltenateKey"/> implementation.
        /// Otherwise, will throw exception.
        /// </i>
        /// </summary>
        /// <param name="uuid">target alternate key</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        Task<TEntity> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        Task<TEntity> GetByUuidTrackingAsync(Guid uuid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the first entity in collection.
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        Task<TEntity> FirstAsync();

        /// <summary>
        /// <para>
        /// Get the first entity in collection.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        Task<TEntity> FirstTrackingAsync();

        /// <summary>
        /// Get the last entity in collection.
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        Task<TEntity> LastAsync();

        /// <summary>
        /// <para>
        /// Get the last entity in collection.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        Task<TEntity> LastTrackingAsync();

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="index">item index on persistence base, from 0</param>
        /// <param name="count">entity count by page list</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        Task<List<TEntity>> PagingIndexAsync(int index, int count, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        Task<List<TEntity>> PagingIndexTrackingAsync(int index, int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        Task<List<TEntity>> PagingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        Task<List<TEntity>> PagingTrackingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default);

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="PagingAsync(int, int, CancellationToken)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, list all values possible</returns>
        Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="PagingAsync(int, int, CancellationToken)"/>.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, list all values possible</returns>
        Task<List<TEntity>> ListTrackingAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <para>
        /// Recovery an amount of values sorted by id.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="PagingAsync(int, int, CancellationToken)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="count"> amount of data</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>list values requested sorted and limited to count</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/>task representation with result, value is less or equals zero.
        /// </exception>
        Task<List<TEntity>> TakeAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// <para>
        /// Recovery an amount of values sorted by id.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="PagingAsync(int, int, CancellationToken)"/>.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="count"> amount of data</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>list values requested sorted and limited to count</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/>task representation with result, value is less or equals zero.
        /// </exception>
        Task<List<TEntity>> TakeTrackingAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// <para>
        /// Recovery an sample of values non sorted.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="PagingAsync(int, int, CancellationToken)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="count">amount of data</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>list values requested non sorted and limited to count</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/>task representation with result, value is less or equals zero.
        /// </exception>
        Task<List<TEntity>> SampleAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// <para>
        /// Recovery an sample of values non sorted.
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="PagingAsync(int, int, CancellationToken)"/>.
        /// </i>
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="count">amount of data</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>list values requested non sorted and limited to count</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/>task representation with result, value is less or equals zero.
        /// </exception>
        Task<List<TEntity>> SampleTrackingAsync(int count, CancellationToken cancellationToken = default);
        #endregion

        #region [U]pdate
        /// <summary>
        /// Update entity on persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, updated target entity</returns>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        #endregion

        #region [D]elete
        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, amount of values removed</returns>
        Task<int> DeleteByUuidAsync(IEnumerable<Guid> uuid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, amount of values removed</returns>
        Task<int> DeleteByUuidAsync(Guid[] uuid, CancellationToken cancellationToken);

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <returns>task representation with result, amount of values removed</returns>
        Task<int> DeleteByUuidAsync(params Guid[] uuid);

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        Task<bool> DeleteByUuidAsync(Guid uuid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        Task<bool> DeleteAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        Task<bool> DeleteAsync(TEntity[] entity, CancellationToken cancellationToken);

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        Task<bool> DeleteAsync(params TEntity[] entity);
        #endregion
    }

    /// <summary>
    /// Entity service CRUD Async for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IServiceCrudAsync<TEntity, ID>  : IServiceCrudAsync<TEntity>, IService<TEntity, ID>
        where TEntity : IModel<ID>
    {
        #region [R]ead        
        /// <summary>
        /// Check whether current ID exists on persistence base.<br/>
        /// </summary>
        /// <param name="id">primary key</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true value exists, otherwhise false</returns>
        Task<bool> ExistsAsync(ID id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get entity by primary key.
        /// </summary>
        /// <param name="id">target id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        Task<TEntity> GetAsync(ID id, CancellationToken cancellationToken = default);
        #endregion

        #region [D]elete
        /// <summary>
        /// Attempt to delete values by id.
        /// </summary>
        /// <param name="id">target id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        Task<bool> DeleteAsync(ID id, CancellationToken cancellationToken = default);
        #endregion
    }
}
