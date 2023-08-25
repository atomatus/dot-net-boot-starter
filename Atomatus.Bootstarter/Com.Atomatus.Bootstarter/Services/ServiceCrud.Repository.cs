using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Repositories;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Service CRUD for repository.
    /// </summary>
    /// <typeparam name="TRepository">entity repository type</typeparam>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public abstract class ServiceCrudForRepository<TRepository, TEntity, ID> :
        IServiceCrud<TEntity, ID>, IServiceCrudAsync<TEntity, ID>
        where TRepository : IRepositoryCrud<TEntity, ID>, IRepositoryCrudAsync<TEntity, ID>
        where TEntity : class, IModel<ID>, new()
    {
        private readonly TRepository repository;

        /// <summary>
        /// Construct a Service CRUD using repository.
        /// </summary>
        /// <param name="repository">entity repository instance</param>
        /// <exception cref="ArgumentNullException">throws when repository is null</exception>
        public ServiceCrudForRepository([NotNull] TRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region IServiceCrud
        /// <inheritdoc/>
        public bool Delete(ID id)
        {
            return repository.Delete(id);
        }

        /// <inheritdoc/>
        public bool Delete(IEnumerable<TEntity> entity)
        {
            return repository.Delete(entity);
        }

        /// <inheritdoc/>
        public bool Delete(params TEntity[] entity)
        {
            return repository.Delete(entity);
        }

        /// <inheritdoc/>
        public int DeleteByUuid(IEnumerable<Guid> uuid)
        {
            return repository.DeleteByUuid(uuid);
        }

        /// <inheritdoc/>
        public int DeleteByUuid(params Guid[] uuid)
        {
            return repository.DeleteByUuid(uuid);
        }

        /// <inheritdoc/>
        public bool DeleteByUuid(Guid uuid)
        {
            return repository.DeleteByUuid(uuid);
        }

        /// <inheritdoc/>
        public bool Exists(ID id)
        {
            return repository.Exists(id);
        }

        /// <inheritdoc/>
        public bool Exists(TEntity e)
        {
            return repository.Exists(e);
        }

        /// <inheritdoc/>
        public bool ExistsByUuid(Guid uuid)
        {
            return repository.ExistsByUuid(uuid);
        }

        /// <inheritdoc/>
        public TEntity First()
        {
            return repository.First();
        }

        /// <inheritdoc/>
        public TEntity FirstTracking()
        {
            return repository.FirstTracking();
        }

        /// <inheritdoc/>
        public TEntity Get(ID id)
        {
            return repository.Get(id);
        }

        /// <inheritdoc/>
        public TEntity GetByUuid(Guid uuid)
        {
            return repository.GetByUuid(uuid);
        }

        /// <inheritdoc/>
        public TEntity GetByUuidTracking(Guid uuid)
        {
            return repository.GetByUuidTracking(uuid);
        }

        /// <inheritdoc/>
        public TEntity GetTracking(ID id)
        {
            return repository.GetTracking(id);
        }

        /// <inheritdoc/>
        public TEntity Last()
        {
            return repository.Last();
        }

        /// <inheritdoc/>
        public TEntity LastTracking()
        {
            return repository.LastTracking();
        }

        /// <inheritdoc/>
        public List<TEntity> List()
        {
            return repository.List();
        }

        /// <inheritdoc/>
        public List<TEntity> ListTracking()
        {
            return repository.ListTracking();
        }

        /// <inheritdoc/>
        public List<TEntity> Paging(int page = 0, int limit = -1)
        {
            return repository.Paging(page, limit);
        }

        /// <inheritdoc/>
        public List<TEntity> PagingIndex(int index, int count)
        {
            return repository.PagingIndex(index, count);
        }

        /// <inheritdoc/>
        public List<TEntity> PagingIndexTracking(int index, int count)
        {
            return repository.PagingIndexTracking(index, count);
        }

        /// <inheritdoc/>
        public List<TEntity> PagingTracking(int page = 0, int limit = -1)
        {
            return repository.PagingTracking(page, limit);
        }

        /// <inheritdoc/>
        public List<TEntity> Sample(int count)
        {
            return repository.Sample(count);
        }

        /// <inheritdoc/>
        public List<TEntity> SampleTracking(int count)
        {
            return repository.SampleTracking(count);
        }

        /// <inheritdoc/>
        public TEntity Save(TEntity entity)
        {
            return repository.Save(entity);
        }

        /// <inheritdoc/>
        public List<TEntity> Take(int count)
        {
            return repository.Take(count);
        }

        /// <inheritdoc/>
        public List<TEntity> TakeTracking(int count)
        {
            return repository.TakeTracking(count);
        }

        /// <inheritdoc/>
        public TEntity Update(TEntity entity)
        {
            return repository.Update(entity);
        }
        #endregion

        #region IServiceCrudAsync
        /// <inheritdoc/>
        public Task<bool> DeleteAsync(ID id, CancellationToken cancellationToken = default)
        {
            return repository.DeleteAsync(id, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> DeleteAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default)
        {
            return repository.DeleteAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> DeleteAsync(TEntity[] entity, CancellationToken cancellationToken)
        {
            return repository.DeleteAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> DeleteAsync(params TEntity[] entity)
        {
            return repository.DeleteAsync(entity);
        }

        /// <inheritdoc/>
        public Task<int> DeleteByUuidAsync(IEnumerable<Guid> uuid, CancellationToken cancellationToken = default)
        {
            return repository.DeleteByUuidAsync(uuid, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<int> DeleteByUuidAsync(Guid[] uuid, CancellationToken cancellationToken)
        {
            return repository.DeleteByUuidAsync(uuid, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<int> DeleteByUuidAsync(params Guid[] uuid)
        {
            return repository.DeleteByUuidAsync(uuid);
        }

        /// <inheritdoc/>
        public Task<bool> DeleteByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return repository.DeleteByUuidAsync(uuid, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(ID id, CancellationToken cancellationToken = default)
        {
            return repository.ExistsAsync(id, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(TEntity e, CancellationToken cancellationToken = default)
        {
            return repository.ExistsAsync(e, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> ExistsByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return repository.ExistsByUuidAsync(uuid, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> FirstAsync()
        {
            return repository.FirstAsync();
        }

        /// <inheritdoc/>
        public Task<TEntity> FirstTrackingAsync()
        {
            return repository.FirstTrackingAsync();
        }

        /// <inheritdoc/>
        public Task<TEntity> GetAsync(ID id, CancellationToken cancellationToken = default)
        {
            return repository.GetAsync(id, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return repository.GetByUuidAsync(uuid, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> GetByUuidTrackingAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            return repository.GetByUuidTrackingAsync(uuid, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> LastAsync()
        {
            return repository.LastAsync();
        }

        /// <inheritdoc/>
        public Task<TEntity> LastTrackingAsync()
        {
            return repository.LastTrackingAsync();
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        {
            return repository.ListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> ListTrackingAsync(CancellationToken cancellationToken = default)
        {
            return repository.ListTrackingAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> PagingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            return repository.PagingAsync(page, limit, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> PagingIndexAsync(int index, int count, CancellationToken cancellationToken = default)
        {
            return repository.PagingIndexAsync(index, count, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> PagingIndexTrackingAsync(int index, int count, CancellationToken cancellationToken = default)
        {
            return repository.PagingIndexTrackingAsync(index, count, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> PagingTrackingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            return repository.PagingTrackingAsync(page, limit, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> SampleAsync(int count, CancellationToken cancellationToken = default)
        {
            return repository.SampleAsync(count, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> SampleTrackingAsync(int count, CancellationToken cancellationToken = default)
        {
            return repository.SampleTrackingAsync(count, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return repository.SaveAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> TakeAsync(int count, CancellationToken cancellationToken = default)
        {
            return repository.TakeAsync(count, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> TakeTrackingAsync(int count, CancellationToken cancellationToken = default)
        {
            return repository.TakeTrackingAsync(count, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return repository.UpdateAsync(entity, cancellationToken);
        }
        #endregion
    }
}
