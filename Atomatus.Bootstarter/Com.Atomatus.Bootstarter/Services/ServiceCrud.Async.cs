using Com.Atomatus.Bootstarter.Context;
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
        where TEntity : ModelBase<ID>, new()
        where TContext : ContextBase
    {
        #region [C]reate
        /// <summary>
        /// Insert a new valeu to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with entity</returns>
        /// <exception cref="ArgumentNullException">throws when entity is null</exception>
        public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await dbSet.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)), cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            OnInsertedCallback(entity);
            return entity;
        }
        #endregion

        #region [R]ead
        /// <summary>
        /// Check whether current uuid exists on persistence base.
        /// </summary>
        /// <param name="uuid">alternate key uuid</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true value exists, otherwhise false</returns>
        public Task<bool> ExistsAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return dbSet
                .AsNoTracking()
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
            return dbSet
                .AsNoTracking()
                .AnyAsync(c => c.Uuid == e.Uuid, cancellationToken);
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
        /// Get entity by alternate key.
        /// </summary>
        /// <param name="uuid">target alternate key</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public Task<TEntity> GetAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Uuid == uuid)
                .OrderBy(e => e.Id)
                .Take(1)
                .FirstOrDefaultAsync(cancellationToken);
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
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found value, otherwhise empty list.</returns>
        public Task<List<TEntity>> PagingAsync(int page, int limit, CancellationToken cancellationToken)
        {
            page = page < 0 ? 0 : page;
            limit = limit <= 0 ? IService<TEntity, ID>.REQUEST_LIST_LIMIT: limit;
            int index = page * limit;//to skip.
            return PagingIndexAsync(index, limit, cancellationToken);
        }

        /// <summary>
        /// List all values in database (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, list all values possible</returns>
        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken)
        {
            return PagingIndexAsync(0, IService<TEntity, ID>.REQUEST_LIST_LIMIT, cancellationToken);
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
            else if (Objects.Compare(entity.Id, default) && Objects.Compare(entity.Uuid, default))
            {
                throw new InvalidOperationException($"Entity \"{typeof(TEntity).Name}\" is untrackable, " +
                    "thus can not be updated! " +
                    "Because it does not contains an Id or Uuid.");
            }

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
                curr = await dbSet
                   .Where(t => t.Uuid == entity.Uuid)
                   .OrderBy(e => e.Id)
                   .Take(1)
                   .FirstOrDefaultAsync(cancellationToken);

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

            await dbContext.SaveChangesAsync(cancellationToken);
            OnUpdatedCallback(entity);
            return entity;
        }
        #endregion

        #region [D]elete
        private async Task<List<TEntity>> AttachRangeNonExistsAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken)
        {
            List<TEntity> result = new List<TEntity>();

            foreach (TEntity e in entity)
            {
                TEntity curr = dbSet.Local.FirstOrDefault(e.EqualsAnyId);
                if (curr != null || (Objects.Compare(e.Id, default) && (curr = await GetAsync(e.Uuid, cancellationToken)) != null))
                {
                    result.Add(curr);
                }
                else if (Objects.Compare(e.Id, default))
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
                    result.Add(e);
                }
            }

            return result;
        }

        private async Task<int> DeleteLocalAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            var aux = uuids.Select(i => new TEntity() { Uuid = i });
            var entity = await AttachRangeNonExistsAsync(aux, cancellationToken);
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
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuids">uuids target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public Task<int> DeleteAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(uuids, cancellationToken);
        }

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="args">uuids target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public Task<int> DeleteAsync(Guid[] args, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(args, cancellationToken);
        }

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="args">uuids target</param>
        /// <returns>task representation with result, amount of values removed</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public Task<int> DeleteAsync(params Guid[] args)
        {
            return DeleteLocalAsync(args, default);
        }

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        /// <exception cref="InvalidOperationException">throws when entity is untrackable, does not contains valid id and Uuid.</exception>
        public Task<bool> DeleteAsync(Guid uuid, CancellationToken cancellationToken)
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
