using Com.Atomatus.Bootstarter.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Services
{
    public interface IServiceCrudAsync<TEntity, ID>  : IService<TEntity, ID>
        where TEntity : IModel<ID>
    {
        #region [C]reate
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        #endregion

        #region [R]ead
        Task<bool> ExistsAsync(Guid uuid, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(TEntity e, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(ID id, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(Guid uuid, CancellationToken cancellationToken = default);

        Task<List<TEntity>> PagingIndexAsync(int index, int count, CancellationToken cancellationToken = default);

        Task<List<TEntity>> PagingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default);

        Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default);
        #endregion

        #region [U]pdate
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        #endregion

        #region [D]elete
        Task<int> DeleteAsync(IEnumerable<Guid> uuid, CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(Guid[] uuid, CancellationToken cancellationToken);

        Task<int> DeleteAsync(params Guid[] uuid);

        Task<bool> DeleteAsync(Guid uuid, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(TEntity[] entity, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(params TEntity[] entity);
        #endregion
    }
}
