using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Services
{
    public abstract partial class ServiceCrud<TContext, TEntity, ID> : IServiceCrudAsync<TEntity, ID>
    {
        #region [C]reate
        /// <summary>
        /// Insert a new valeu to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with entity</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        public async Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            RequireValidate(entity);
            await dbSet.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)), cancellationToken);
            
            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
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
            this.RequireEntityImplementIModelAlternateKey();
            return dbSet
                .AsNoTracking()
                .OfType<IModelAltenateKey>()
                .AnyAsync(e => e.Uuid == uuid, cancellationToken);
        }

        /// <summary>
        /// Check whether current entity exists on persistence base.
        /// </summary>
        /// <param name="e">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true value exists, otherwhise false</returns>
        public Task<bool> ExistsAsync(TEntity e, CancellationToken cancellationToken)
        {
            if (e is IModelAltenateKey eAlt)
            {
                return dbSet
                    .AsNoTracking()
                    .OfType<IModelAltenateKey>()
                    .AnyAsync(c => c.Uuid == eAlt.Uuid, cancellationToken);
            }
            else
            {
                return dbSet
                    .AsNoTracking()
                    .AnyAsync(c => c.Id.Equals(e.Id), cancellationToken);
            }
        }

        /// <summary>
        /// Check whether current ID exists on persistence base.<br/>
        /// </summary>
        /// <param name="id">primary key</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true value exists, otherwhise false</returns>
        public Task<bool> ExistsAsync(ID id, CancellationToken cancellationToken = default)
        {
            return dbSet
                    .AsNoTracking()
                    .AnyAsync(c => c.Id.Equals(id), cancellationToken);
        }

        /// <summary>
        /// Get entity by primary key.
        /// </summary>
        /// <param name="id">target id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public async Task<TEntity> GetAsync(ID id, CancellationToken cancellationToken)
        {
            TEntity found = await dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (found != null) dbContext.Entry(found).State = EntityState.Detached;
            return found;
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
            return dbSet
                .AsNoTracking()
                .OfType<IModelAltenateKey>()
                .Where(t => t.Uuid == uuid)
                .OfType<TEntity>()
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefaultAsync(cancellationToken);
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
            return dbSet
                .OfType<IModelAltenateKey>()
                .Where(t => t.Uuid == uuid)
                .OfType<TEntity>()
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Get the first entity in collection.
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public Task<TEntity> FirstAsync()
        {
            return dbSet
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefaultAsync();
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
            return dbSet
                .OrderBy(t => t.Id)
                .Take(1)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get the last entity in collection.
        /// </summary>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public Task<TEntity> LastAsync()
        {
            return dbSet
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefaultAsync();
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
            return dbSet
                .OrderByDescending(t => t.Id)
                .Take(1)
                .FirstOrDefaultAsync();
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
                .ToListAsync(cancellationToken);
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
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        public Task<List<TEntity>> PagingAsync(int page, int limit, CancellationToken cancellationToken)
        {
            limit = limit <= 0 ? IService<TEntity, ID>.REQUEST_LIST_LIMIT: limit;
            int index = Math.Max(page, 0) * limit;//to skip.
            return PagingIndexAsync(index, limit, cancellationToken);
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
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        public Task<List<TEntity>> PagingTrackingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            limit = limit <= 0 ? IService<TEntity, ID>.REQUEST_LIST_LIMIT : limit;
            int index = Math.Max(page, 0) * limit;//to skip.
            return PagingIndexTrackingAsync(index, limit, cancellationToken);
        }

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
        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken)
        {
            return PagingIndexAsync(0, IService<TEntity, ID>.REQUEST_LIST_LIMIT, cancellationToken);
        }

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
        public Task<List<TEntity>> ListTrackingAsync(CancellationToken cancellationToken = default)
        {
            return PagingIndexTrackingAsync(0, IService<TEntity, ID>.REQUEST_LIST_LIMIT, cancellationToken);
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
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            //oh! a "Take" request without "OrderBy", it's ok!
            //check de method message it is the purpose.
            return this.dbSet
                .AsNoTracking()
                .Take(count)
                .ToListAsync();
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
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            //oh! a "Take" request without "OrderBy", it's ok!
            //check de method message it is the purpose.
            return this.dbSet
                .Take(count)
                .ToListAsync();
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
        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
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
            RequireValidate(entity);
            //check contains in tracking local dbSet.
            TEntity curr = dbSet.Local
                .AsParallel()
                .FirstOrDefault(entity.EqualsAnyId);

            if (curr != null)
            {
                dbContext.Entry(curr)
                    .CurrentValues
                    .SetValues(entity);
            }
            else if (Objects.Compare(entity.Id, default))
            {
                IModelAltenateKey altKey = entity as IModelAltenateKey ??
                       throw new InvalidCastException($"Entity \"{typeof(TEntity).Name}\" " +
                           $"does not implements {typeof(IModelAltenateKey).Name}!");

                curr = await dbSet
                   .OfType<IModelAltenateKey>()
                   .Where(t => t.Uuid == altKey.Uuid)
                   .OfType<TEntity>()
                   .OrderBy(t => t.Id)
                   .Take(1)
                   .FirstOrDefaultAsync(cancellationToken);

                if (curr == null)
                {
                    throw new DbUpdateException($"Entity \"{typeof(TEntity).Name}\" " +
                        $"with Uuid \"{altKey.Uuid}\" does not exist on database!");
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

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);

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
        private async Task<List<TEntity>> AttachRangeNonExistsAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            List<TEntity> result = new List<TEntity>();

            foreach(TEntity e in entities)
            {
                TEntity curr = dbSet.Local.FirstOrDefault(e.EqualsAnyId);

                if(curr != null)
                {
                    result.Add(curr);
                }
                else if(Objects.Compare(e.Id, default))
                {
                    if(e is IModelAltenateKey altKey && await GetByUuidAsync(altKey.Uuid, cancellationToken) is TEntity found)
                    {
                        result.Add(found);
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
                    result.Add(e);
                }
            }

            return result;
        }

        private Task<List<TEntity>> AttachRangeNonExistsAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            this.RequireEntityImplementIModelAlternateKey();
            return AttachRangeNonExistsAsync(uuids.Select(uuid =>
            {
                TEntity t = new TEntity { };
                IModelAltenateKey altKey = (IModelAltenateKey)t;
                altKey.Uuid = uuid;
                return t;
            }), cancellationToken);
        }

        private async Task<int> DeleteLocalAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            var entity = await AttachRangeNonExistsAsync(uuids, cancellationToken);
            dbSet.RemoveRange(entity);
            return await dbContext.SaveChangesAsync(cancellationToken)                
                .ContinueWith(t =>
                {
                    int res = Math.Min(t.Result, entity.Count);
                    if (res > 0) OnDeletedCallback(entity);
                    return res;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
        
        /// <summary>
        /// Attempt to delete values by id.
        /// </summary>
        /// <param name="id">target id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        public Task<bool> DeleteAsync(ID id, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(
                new[] { new TEntity { Id = id } }, 
                cancellationToken);
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
        public async Task<bool> DeleteAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken)
        {
            var att = await AttachRangeNonExistsAsync(entity, cancellationToken);
            dbSet.RemoveRange(att);
            var res = await dbContext.SaveChangesAsync(cancellationToken) >= entity.Count();
            if (res) OnDeletedCallback(att);
            return res;
        }

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public async Task<bool> DeleteAsync(TEntity[] entity, CancellationToken cancellationToken)
        {
            var att = await AttachRangeNonExistsAsync(entity, cancellationToken);
            dbSet.RemoveRange(att);
            var res = await dbContext.SaveChangesAsync(cancellationToken) >= entity.Length;
            if (res) OnDeletedCallback(att);
            return res;
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
