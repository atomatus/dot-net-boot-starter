using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Repositories;

namespace Com.Atomatus.Bootstarter.Services
{
    public abstract class ServiceCrudForRepository<TRepository, TEntity, ID> :
        IServiceCrud<TEntity, ID>, IServiceCrudAsync<TEntity, ID>
        where TRepository : IRepositoryCrud<TEntity, ID>, IRepositoryCrudAsync<TEntity, ID>
        where TEntity : class, IModel<ID>, new()
    {
        private readonly TRepository repository;

        public ServiceCrudForRepository([NotNull] TRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region IServiceCrud
        public bool Delete(ID id)
        {
            return repository.Delete(id);
        }

        public bool Delete(IEnumerable<TEntity> entity)
        {
            return repository.Delete(entity);
        }

        public bool Delete(params TEntity[] entity)
        {
            return repository.Delete(entity);
        }

        public int DeleteByUuid(IEnumerable<Guid> uuid)
        {
            return repository.DeleteByUuid(uuid);
        }

        public int DeleteByUuid(params Guid[] uuid)
        {
            return repository.DeleteByUuid(uuid);
        }

        public bool DeleteByUuid(Guid uuid)
        {
            return repository.DeleteByUuid(uuid);
        }

        public bool Exists(ID id)
        {
            return repository.Exists(id);
        }

        public bool Exists(TEntity e)
        {
            return repository.Exists(e);
        }

        public bool ExistsByUuid(Guid uuid)
        {
            return repository.ExistsByUuid(uuid);
        }

        public TEntity First()
        {
            return repository.First();
        }

        public TEntity FirstTracking()
        {
            return repository.FirstTracking();
        }

        public TEntity Get(ID id)
        {
            return repository.Get(id);
        }

        public TEntity GetByUuid(Guid uuid)
        {
            return repository.GetByUuid(uuid);
        }

        public TEntity GetByUuidTracking(Guid uuid)
        {
            return repository.GetByUuidTracking(uuid);
        }

        public TEntity GetTracking(ID id)
        {
            return repository.GetTracking(id);
        }

        public TEntity Last()
        {
            return repository.Last();
        }

        public TEntity LastTracking()
        {
            return repository.LastTracking();
        }

        public List<TEntity> List()
        {
            return repository.List();
        }

        public List<TEntity> ListTracking()
        {
            return repository.ListTracking();
        }

        public List<TEntity> Paging(int page = 0, int limit = -1)
        {
            return repository.Paging(page, limit);
        }

        public List<TEntity> PagingIndex(int index, int count)
        {
            return repository.PagingIndex(index, count);
        }

        public List<TEntity> PagingIndexTracking(int index, int count)
        {
            return repository.PagingIndexTracking(index, count);
        }

        public List<TEntity> PagingTracking(int page = 0, int limit = -1)
        {
            return repository.PagingTracking(page, limit);
        }

        public List<TEntity> Sample(int count)
        {
            return repository.Sample(count);
        }

        public List<TEntity> SampleTracking(int count)
        {
            return repository.SampleTracking(count);
        }

        public TEntity Save(TEntity entity)
        {
            return repository.Save(entity);
        }

        public List<TEntity> Take(int count)
        {
            return repository.Take(count);
        }

        public List<TEntity> TakeTracking(int count)
        {
            return repository.TakeTracking(count);
        }

        public TEntity Update(TEntity entity)
        {
            return repository.Update(entity);
        }
        #endregion

        #region IServiceCrudAsync
        public Task<bool> DeleteAsync(ID id, CancellationToken cancellationToken = default)
        {
            return repository.DeleteAsync(id, cancellationToken);
        }

        public Task<bool> DeleteAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default)
        {
            return repository.DeleteAsync(entity, cancellationToken);
        }

        public Task<bool> DeleteAsync(TEntity[] entity, CancellationToken cancellationToken)
        {
            return repository.DeleteAsync(entity, cancellationToken);
        }

        public Task<bool> DeleteAsync(params TEntity[] entity)
        {
            return repository.DeleteAsync(entity);
        }

        public Task<int> DeleteByUuidAsync(IEnumerable<Guid> uuid, CancellationToken cancellationToken = default)
        {
            return repository.DeleteByUuidAsync(uuid, cancellationToken);
        }

        public Task<int> DeleteByUuidAsync(Guid[] uuid, CancellationToken cancellationToken)
        {
            return repository.DeleteByUuidAsync(uuid, cancellationToken);
        }

        public Task<int> DeleteByUuidAsync(params Guid[] uuid)
        {
            return repository.DeleteByUuidAsync(uuid);
        }

        public Task<bool> DeleteByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return repository.DeleteByUuidAsync(uuid, cancellationToken);
        }

        public Task<bool> ExistsAsync(ID id, CancellationToken cancellationToken = default)
        {
            return repository.ExistsAsync(id, cancellationToken);
        }

        public Task<bool> ExistsAsync(TEntity e, CancellationToken cancellationToken = default)
        {
            return repository.ExistsAsync(e, cancellationToken);
        }

        public Task<bool> ExistsByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return repository.ExistsByUuidAsync(uuid, cancellationToken);
        }

        public Task<TEntity> FirstAsync()
        {
            return repository.FirstAsync();
        }

        public Task<TEntity> FirstTrackingAsync()
        {
            return repository.FirstTrackingAsync();
        }

        public Task<TEntity> GetAsync(ID id, CancellationToken cancellationToken = default)
        {
            return repository.GetAsync(id, cancellationToken);
        }

        public Task<TEntity> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return repository.GetByUuidAsync(uuid, cancellationToken);
        }

        public Task<TEntity> GetByUuidTrackingAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return repository.GetByUuidTrackingAsync(uuid, cancellationToken);
        }

        public Task<TEntity> LastAsync()
        {
            return repository.LastAsync();
        }

        public Task<TEntity> LastTrackingAsync()
        {
            return repository.LastTrackingAsync();
        }

        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        {
            return repository.ListAsync(cancellationToken);
        }

        public Task<List<TEntity>> ListTrackingAsync(CancellationToken cancellationToken = default)
        {
            return repository.ListTrackingAsync(cancellationToken);
        }

        public Task<List<TEntity>> PagingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            return repository.PagingAsync(page, limit, cancellationToken);
        }

        public Task<List<TEntity>> PagingIndexAsync(int index, int count, CancellationToken cancellationToken = default)
        {
            return repository.PagingIndexAsync(index, count, cancellationToken);
        }

        public Task<List<TEntity>> PagingIndexTrackingAsync(int index, int count, CancellationToken cancellationToken = default)
        {
            return repository.PagingIndexTrackingAsync(index, count, cancellationToken);
        }

        public Task<List<TEntity>> PagingTrackingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            return repository.PagingTrackingAsync(page, limit, cancellationToken);
        }

        public Task<List<TEntity>> SampleAsync(int count, CancellationToken cancellationToken = default)
        {
            return repository.SampleAsync(count, cancellationToken);
        }

        public Task<List<TEntity>> SampleTrackingAsync(int count, CancellationToken cancellationToken = default)
        {
            return repository.SampleTrackingAsync(count, cancellationToken);
        }

        public Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return repository.SaveAsync(entity, cancellationToken);
        }

        public Task<List<TEntity>> TakeAsync(int count, CancellationToken cancellationToken = default)
        {
            return repository.TakeAsync(count, cancellationToken);
        }

        public Task<List<TEntity>> TakeTrackingAsync(int count, CancellationToken cancellationToken = default)
        {
            return repository.TakeTrackingAsync(count, cancellationToken);
        }

        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return repository.UpdateAsync(entity, cancellationToken);
        }
        #endregion
    }
}
