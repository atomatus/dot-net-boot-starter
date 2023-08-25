using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter
{
    public abstract partial class Crud<TContext, TEntity> : ICrudAsync<TEntity>
    {
        #region [C]reate
        /// <summary>
        /// Insert a new valeu to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with entity</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        public Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => Save(entity), cancellationToken);
        }
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
        public Task<bool> ExistsByUuidAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => ExistsByUuid(uuid), cancellationToken);
        }

        /// <summary>
        /// Check whether current entity exists on persistence base.
        /// </summary>
        /// <param name="e">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true value exists, otherwhise false</returns>
        public Task<bool> ExistsAsync(TEntity e, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => Exists(e), cancellationToken);
        }

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
        public Task<TEntity> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => GetByUuid(uuid), cancellationToken);
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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public Task<TEntity> GetByUuidTrackingAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() => GetByUuidTracking(uuid), cancellationToken);
        }

        /// <summary>
        /// Get the first entity in collection.
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public Task<TEntity> FirstAsync()
        {
            return Task.Factory.StartNew(First);
        }

        /// <summary>
        /// <para>
        /// Get the first entity in collection.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public Task<TEntity> FirstTrackingAsync()
        {
            return Task.Factory.StartNew(FirstTracking);
        }

        /// <summary>
        /// Get the last entity in collection.
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public Task<TEntity> LastAsync()
        {
            return Task.Factory.StartNew(Last);
        }

        /// <summary>
        /// <para>
        /// Get the last entity in collection.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public Task<TEntity> LastTrackingAsync()
        {
            return Task.Factory.StartNew(LastTracking);
        }

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="index">item index on persistence base, from 0</param>
        /// <param name="count">entity count by page list</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> value is less then zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> value is less or equals zero.
        /// </exception>
        public Task<List<TEntity>> PagingIndexAsync(int index, int count, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => PagingIndex(index, count), cancellationToken);
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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        public Task<List<TEntity>> PagingIndexTrackingAsync(int index, int count, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() => PagingIndexTracking(index, count), cancellationToken);
        }

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>).</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        public Task<List<TEntity>> PagingAsync(int page, int limit, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => Paging(page, limit), cancellationToken);
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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        public Task<List<TEntity>> PagingTrackingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() => PagingTracking(page, limit), cancellationToken);
        }

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
        /// </para>
        /// <para>
        /// <i>
        /// Warning: For a better performing in amount of data large use <see cref="PagingAsync(int, int, CancellationToken)"/>.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, list all values possible</returns>
        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken)
        {
            return PagingIndexAsync(0, ICrud<TEntity>.REQUEST_LIST_LIMIT, cancellationToken);
        }

        /// <summary>
        /// <para>
        /// List all values in database (limited to max request <see cref="ICrud{TEntity}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
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
        public Task<List<TEntity>> ListTrackingAsync(CancellationToken cancellationToken = default)
        {
            return PagingIndexTrackingAsync(0, ICrud<TEntity>.REQUEST_LIST_LIMIT, cancellationToken);
        }

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
        public Task<List<TEntity>> TakeAsync(int count, CancellationToken cancellationToken)
        {
            return PagingIndexAsync(0, count, cancellationToken);
        }

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
        public Task<List<TEntity>> TakeTrackingAsync(int count, CancellationToken cancellationToken = default)
        {
            return PagingIndexTrackingAsync(0, count, cancellationToken);
        }

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
        public Task<List<TEntity>> SampleAsync(int count, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => Sample(count), cancellationToken);
        }

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
        public Task<List<TEntity>> SampleTrackingAsync(int count, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() => SampleTracking(count), cancellationToken);
        }
        #endregion

        #region [U]pdate
        /// <summary>
        /// Update entity on persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, updated target entity</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="DbUpdateException">throws when is not possible update value, value does not exists, for example.</exception>
        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => Update(entity), cancellationToken);
        }
        #endregion

        #region [D]elete
        private Task<int> DeleteLocalAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => DeleteLocal(uuids), cancellationToken);
        }

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuids">uuids target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public Task<int> DeleteByUuidAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(uuids, cancellationToken);
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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public Task<int> DeleteByUuidAsync(Guid[] args, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(args, cancellationToken);
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
        /// <returns>task representation with result, amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public Task<int> DeleteByUuidAsync(params Guid[] args)
        {
            return DeleteLocalAsync(args, default);
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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        /// <exception cref="InvalidCastException">
        /// Throws exception when <typeparamref name="TEntity"/>
        /// does not contains <see cref="IModelAltenateKey"/> implementated it.
        /// </exception>
        public Task<bool> DeleteByUuidAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(new Guid[] { uuid }, cancellationToken).ContinueWith(t => t.Result == 1);
        }

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public Task<bool> DeleteAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => Delete(entity), cancellationToken);
        }

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public Task<bool> DeleteAsync(TEntity[] entity, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => Delete(entity), cancellationToken);
        }

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public Task<bool> DeleteAsync(params TEntity[] entity)
        {
            return DeleteAsync(entity, default);
        }
        #endregion
    }
}
