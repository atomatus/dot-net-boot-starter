using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// CRUD Operation for target entity by ID.
    /// </summary>
    /// <typeparam name="TEntity">target entity</typeparam>
    /// <typeparam name="ID">target entity id type</typeparam>
    public interface ICrud<TEntity, ID> : ICrud<TEntity>
        where TEntity : IModel<ID>
    {
        #region [R]ead      
        /// <summary>
        /// Check whether current ID exists on persistence base.<br/>
        /// </summary>
        /// <param name="id">primary key</param>
        /// <returns>true, value exists, otherwhise false</returns>
        bool Exists(ID id);

        /// <summary>
        /// Get entity by primary key.
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>found entity, otherwise null value</returns>
        TEntity Get(ID id);

        /// <summary>
        /// <para>
        /// Get entity by primary key.
        /// </para>
        /// <para>
        /// Obs.: This request is Tracking enabled.
        /// </para>
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>found entity, otherwise null value</returns>
        TEntity GetTracking(ID id);
        #endregion

        #region [D]elete
        /// <summary>
        /// Attempt to delete values by id.<br/>
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>value removed</returns>
        bool Delete(ID id);
        #endregion
    }
}
