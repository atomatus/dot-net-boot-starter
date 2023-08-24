using System.Threading;
using System.Threading.Tasks;
using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// CRUD Operation Async for target entity by ID.
    /// </summary>
    /// <typeparam name="TEntity">target entity</typeparam>
    /// <typeparam name="ID">target entity id type</typeparam>
	public interface ICrudAsync<TEntity, ID> : ICrudAsync<TEntity>
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
