using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Services
{
    public abstract partial class ServiceCrud<TContext, TEntity, ID> : IServiceCrud<TEntity, ID>
        where TEntity : ModelBase<ID>, new()
        where TContext : ContextBase
    {
        private const int REQUEST_LIST_LIMIT = 300;

        protected readonly TContext dbContext;
        protected readonly DbSet<TEntity> dbSet;

        public ServiceCrud([NotNull] TContext context, [NotNull] DbSet<TEntity> dbSet)
        {
            this.dbContext  = context ?? throw new ArgumentNullException(nameof(context));
            this.dbSet      = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        }

        public ServiceCrud([NotNull] TContext context) : this(context, context?.GetOrSet<TEntity>()) { }

        #region [C]reate
        public TEntity Insert(TEntity entity)
        {
            dbSet.Add(entity ?? throw new ArgumentNullException(nameof(entity)));
            dbContext.SaveChanges();
            OnInsertedCallback(entity);
            return entity;
        }
        #endregion

        #region [R]ead        
        public bool Exists(Guid uuid)
        {
            return dbSet
                .AsNoTracking()
                .Any(e => e.Uuid == uuid);
        }

        public bool Exists(TEntity e)
        {
            return dbSet
                .AsNoTracking()
                .Any(c => c.Uuid == e.Uuid);
        }

        public TEntity Get(ID id)
        {
            TEntity found = dbSet.Find(id);
            if (found != null) dbContext.Entry(found).State = EntityState.Detached;
            return found;
        }

        public TEntity GetByUuid(Guid uuid)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Uuid == uuid)
                .OrderBy(e => e.Id)
                .Take(1)
                .FirstOrDefault();
        }

        public List<TEntity> PagingIndex(int index, int count)
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
                .ToList();
        }

        public List<TEntity> Paging(int page, int limit)
        {
            page = page < 0 ? 0 : page;
            limit = limit <= 0 ? REQUEST_LIST_LIMIT : limit;
            int index = page * limit;//to skip.
            return PagingIndex(index, limit);
        }

        public List<TEntity> List()
        {
            return PagingIndex(0, REQUEST_LIST_LIMIT);
        }
        #endregion
        
        #region [U]pdate
        public TEntity Update(TEntity entity)
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

            TEntity curr = dbSet.Local.FirstOrDefault(entity.EqualsAnyId);

            if (curr != null)
            {
                dbContext.Entry(curr)
                    .CurrentValues
                    .SetValues(entity);
            }
            else if(Objects.Compare(entity.Id, default))
            {
                curr = dbSet
                   .Where(t => t.Uuid == entity.Uuid)
                   .OrderBy(e => e.Id)
                   .Take(1)
                   .FirstOrDefault();

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

            dbContext.SaveChanges();
            OnUpdatedCallback(entity);
            return entity;
        }
        #endregion

        #region [D]elete
        private IEnumerable<TEntity> AttachRangeNonExists(IEnumerable<TEntity> entity)
        {
            foreach(TEntity e in entity)
            {
                TEntity curr = dbSet.Local.FirstOrDefault(e.EqualsAnyId);                
                if(curr != null || (Objects.Compare(e.Id, default) && (curr = GetByUuid(e.Uuid)) != null))
                {
                    yield return curr;
                }
                else if(Objects.Compare(e.Id, default))
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
                    yield return e;
                }
            }
        }

        private int DeleteLocal(IEnumerable<Guid> uuids)
        {
            var aux     = uuids.Select(i => new TEntity() { Uuid = i });
            var entity  = AttachRangeNonExists(aux).ToList();
            dbSet.RemoveRange(entity);
            int count   = dbContext.SaveChanges();
            if(count > 0) OnDeletedCallback(entity);
            return Math.Min(count, entity.Count);
        }

        public int Delete(IEnumerable<Guid> uuids)
        {
            return DeleteLocal(uuids);
        }

        public int Delete(params Guid[] args)
        {
            return DeleteLocal(args);
        }

        public bool Delete(Guid uuid)
        {
            return DeleteLocal(new Guid[] { uuid }) == 1;
        }

        public bool Delete(IEnumerable<TEntity> entity)
        {
            var att = AttachRangeNonExists(entity);
            dbSet.RemoveRange(att);
            var res = dbContext.SaveChanges() >= entity.Count();
            if (res) OnDeletedCallback(att);
            return res;
        }

        public bool Delete(params TEntity[] entity)
        {
            var att = AttachRangeNonExists(entity);
            dbSet.RemoveRange(att);
            var res = dbContext.SaveChanges() >= entity.Length;
            if (res) OnDeletedCallback(att);
            return res;
        }
        #endregion

        #region Callbacks
        protected virtual void OnInsertedCallback(TEntity entity) { }

        protected virtual void OnUpdatedCallback(TEntity entity) { }

        protected virtual void OnDeletedCallback(IEnumerable<TEntity> entities) { }
        #endregion

    }
}
