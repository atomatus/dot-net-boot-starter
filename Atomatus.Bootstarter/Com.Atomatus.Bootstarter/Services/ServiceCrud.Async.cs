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
        public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await dbSet.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)), cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            OnInsertedCallback(entity);
            return entity;
        }
        #endregion

        #region [R]ead
        public Task<bool> ExistsAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return dbSet
                .AsNoTracking()
                .AnyAsync(e => e.Uuid == uuid, cancellationToken);
        }

        public Task<bool> ExistsAsync(TEntity e, CancellationToken cancellationToken)
        {
            return dbSet
                .AsNoTracking()
                .AnyAsync(c => c.Uuid == e.Uuid, cancellationToken);
        }

        public async Task<TEntity> GetAsync(ID id, CancellationToken cancellationToken)
        {
            TEntity found = await dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (found != null) dbContext.Entry(found).State = EntityState.Detached;
            return found;
        }

        public Task<TEntity> GetAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Uuid == uuid)
                .OrderBy(e => e.Id)
                .Take(1)
                .FirstOrDefaultAsync(cancellationToken);
        }

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

        public Task<List<TEntity>> PagingAsync(int page, int limit, CancellationToken cancellationToken)
        {
            page = page < 0 ? 0 : page;
            limit = limit <= 0 ? REQUEST_LIST_LIMIT : limit;
            int index = page * limit;//to skip.
            return PagingIndexAsync(index, limit, cancellationToken);
        }

        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken)
        {
            return PagingIndexAsync(0, REQUEST_LIST_LIMIT, cancellationToken);
        }
        #endregion
        
        #region [U]pdate
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
                    throw new InvalidOperationException($"Entity \"{typeof(TEntity).Name}\" with Uuid \"{entity.Uuid}\" does not exist on database!");
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

        public Task<int> DeleteAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(uuids, cancellationToken);
        }

        public Task<int> DeleteAsync(Guid[] args, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(args, cancellationToken);
        }

        public Task<int> DeleteAsync(params Guid[] args)
        {
            return DeleteLocalAsync(args, default);
        }

        public Task<bool> DeleteAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(new Guid[] { uuid }, cancellationToken).ContinueWith(t => t.Result == 1);
        }

        public async Task<bool> DeleteAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken)
        {
            var att = await AttachRangeNonExistsAsync(entity, cancellationToken);
            dbSet.RemoveRange(att);
            var res = await dbContext.SaveChangesAsync(cancellationToken) >= entity.Count();
            if (res) OnDeletedCallback(att);
            return res;
        }

        public async Task<bool> DeleteAsync(TEntity[] entity, CancellationToken cancellationToken)
        {
            var att = await AttachRangeNonExistsAsync(entity, cancellationToken);
            dbSet.RemoveRange(att);
            var res = await dbContext.SaveChangesAsync(cancellationToken) >= entity.Length;
            if (res) OnDeletedCallback(att);
            return res;
        }

        public Task<bool> DeleteAsync(params TEntity[] entity)
        {
            return DeleteAsync(entity, default);
        }
        #endregion
    }
}
