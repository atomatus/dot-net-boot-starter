using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// CRUD Operations for DBContext/DBSet using ID.
    /// </summary>
    /// <typeparam name="TContext">context type</typeparam>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID"></typeparam>
    public abstract partial class CrudId<TContext, TEntity, ID> : ICrudAsync<TEntity, ID>
    {
        #region [R]ead
        /// <summary>
        /// Check whether current ID exists on persistence base.<br/>
        /// </summary>
        /// <param name="id">primary key</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true value exists, otherwhise false</returns>
        public Task<bool> ExistsAsync(ID id, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() => Exists(id), cancellationToken);
        }

        /// <summary>
        /// Get entity by primary key.
        /// </summary>
        /// <param name="id">target id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, found entity, otherwise null value</returns>
        public Task<TEntity> GetAsync(ID id, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => Get(id), cancellationToken);
        }
        #endregion

        #region [D]elete
        /// <summary>
        /// Attempt to delete values by id.
        /// </summary>
        /// <param name="id">target id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>task representation with result, true, removed value, otherwhise false.</returns>
        public Task<bool> DeleteAsync(ID id, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() => Delete(id), cancellationToken);
        }
        #endregion
    }
}
