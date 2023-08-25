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
        where TRepository : IRepositoryCrud<TEntity, ID>
        where TEntity : class, IModel<ID>, new()
    {
        private readonly TRepository repository;

        /// <summary>
        /// Current <typeparamref name="TRepository"/> reference.
        /// </summary>
        protected TRepository Repository
        {
            get => repository;
        }

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
            if (repository is IRepositoryCrudAsync<TEntity,ID> repositoryAsync)
            {
                return repositoryAsync.DeleteAsync(id, cancellationToken);
            }

            return Task.Factory.StartNew(() => Delete(id));
        }

        /// <inheritdoc/>
        public Task<bool> DeleteAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.DeleteAsync(entity, cancellationToken);
            }

            return Task.Factory.StartNew(() => Delete(entity));
        }

        /// <inheritdoc/>
        public Task<bool> DeleteAsync(TEntity[] entity, CancellationToken cancellationToken)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.DeleteAsync(entity, cancellationToken);
            }

            return Task.Factory.StartNew(() => Delete(entity));
        }

        /// <inheritdoc/>
        public Task<bool> DeleteAsync(params TEntity[] entity)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.DeleteAsync(entity);
            }

            return Task.Factory.StartNew(() => Delete(entity));
        }

        /// <inheritdoc/>
        public Task<int> DeleteByUuidAsync(IEnumerable<Guid> uuid, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.DeleteByUuidAsync(uuid, cancellationToken);
            }

            return Task.Factory.StartNew(() => DeleteByUuid(uuid));
        }

        /// <inheritdoc/>
        public Task<int> DeleteByUuidAsync(Guid[] uuid, CancellationToken cancellationToken)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.DeleteByUuidAsync(uuid, cancellationToken);
            }

            return Task.Factory.StartNew(() => DeleteByUuid(uuid));
        }

        /// <inheritdoc/>
        public Task<int> DeleteByUuidAsync(params Guid[] uuid)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.DeleteByUuidAsync(uuid);
            }

            return Task.Factory.StartNew(() => DeleteByUuid(uuid));
        }

        /// <inheritdoc/>
        public Task<bool> DeleteByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.DeleteByUuidAsync(uuid, cancellationToken);
            }

            return Task.Factory.StartNew(() => DeleteByUuid(uuid));
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(ID id, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.ExistsAsync(id, cancellationToken);
            }

            return Task.Factory.StartNew(() => Exists(id));
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(TEntity e, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.ExistsAsync(e, cancellationToken);
            }

            return Task.Factory.StartNew(() => Exists(e));
        }

        /// <inheritdoc/>
        public Task<bool> ExistsByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.ExistsByUuidAsync(uuid, cancellationToken);
            }

            return Task.Factory.StartNew(() => ExistsByUuid(uuid));
        }

        /// <inheritdoc/>
        public Task<TEntity> FirstAsync()
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.FirstAsync();
            }

            return Task.Factory.StartNew(First);
        }

        /// <inheritdoc/>
        public Task<TEntity> FirstTrackingAsync()
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.FirstTrackingAsync();
            }

            return Task.Factory.StartNew(FirstTracking);
        }

        /// <inheritdoc/>
        public Task<TEntity> GetAsync(ID id, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.GetAsync(id, cancellationToken);
            }

            return Task.Factory.StartNew(() => Get(id));
        }

        /// <inheritdoc/>
        public Task<TEntity> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.GetByUuidAsync(uuid, cancellationToken);
            }

            return Task.Factory.StartNew(() => GetByUuid(uuid));
        }

        /// <inheritdoc/>
        public Task<TEntity> GetByUuidTrackingAsync(Guid uuid, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.GetByUuidTrackingAsync(uuid, cancellationToken);
            }

            return Task.Factory.StartNew(() => GetByUuidTracking(uuid));
        }

        /// <inheritdoc/>
        public Task<TEntity> LastAsync()
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.LastAsync();
            }

            return Task.Factory.StartNew(Last);
        }

        /// <inheritdoc/>
        public Task<TEntity> LastTrackingAsync()
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.LastTrackingAsync();
            }

            return Task.Factory.StartNew(LastTracking);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.ListAsync(cancellationToken);
            }

            return Task.Factory.StartNew(List);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> ListTrackingAsync(CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.ListTrackingAsync(cancellationToken);
            }

            return Task.Factory.StartNew(ListTracking);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> PagingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.PagingAsync(page, limit, cancellationToken);
            }

            return Task.Factory.StartNew(() => Paging(page, limit));
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> PagingIndexAsync(int index, int count, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.PagingIndexAsync(index, count, cancellationToken);
            }

            return Task.Factory.StartNew(() => PagingIndex(index, count));
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> PagingIndexTrackingAsync(int index, int count, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.PagingIndexTrackingAsync(index, count, cancellationToken);
            }

            return Task.Factory.StartNew(() => PagingIndexTracking(index, count));
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> PagingTrackingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.PagingTrackingAsync(page, limit, cancellationToken);
            }

            return Task.Factory.StartNew(() => PagingTracking(page, limit));
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> SampleAsync(int count, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.SampleAsync(count, cancellationToken);
            }

            return Task.Factory.StartNew(() => Sample(count));
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> SampleTrackingAsync(int count, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.SampleTrackingAsync(count, cancellationToken);
            }

            return Task.Factory.StartNew(() => SampleTracking(count));
        }

        /// <inheritdoc/>
        public Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.SaveAsync(entity, cancellationToken);
            }

            return Task.Factory.StartNew(() => Save(entity));
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> TakeAsync(int count, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.TakeAsync(count, cancellationToken);
            }

            return Task.Factory.StartNew(() => Take(count));
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> TakeTrackingAsync(int count, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.TakeTrackingAsync(count, cancellationToken);
            }

            return Task.Factory.StartNew(() => TakeTracking(count));
        }

        /// <inheritdoc/>
        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (repository is IRepositoryCrudAsync<TEntity, ID> repositoryAsync)
            {
                return repositoryAsync.UpdateAsync(entity, cancellationToken);
            }

            return Task.Factory.StartNew(() => Update(entity));
        }
        #endregion
    }
}
