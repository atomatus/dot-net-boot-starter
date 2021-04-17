using Com.Atomatus.Bootstarter.Model;
using System;
using System.Collections.Generic;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Entity service CRUD for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IServiceCrud<TEntity, ID> : IService<TEntity, ID>
        where TEntity : IModel<ID>
    {
        #region [C]reate
        /// <summary>
        /// Insert a new valeu to persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <returns>entity within ids</returns>
        TEntity Insert(TEntity entity);
        #endregion

        #region [R]ead
        /// <summary>
        /// Check whether current uuid exists on persistence base.
        /// </summary>
        /// <param name="uuid">alternate key uuid</param>
        /// <returns>true, value exists, otherwhise false</returns>
        bool Exists(Guid uuid);

        /// <summary>
        /// Check whether current entity exists on persistence base.
        /// </summary>
        /// <param name="e">target entity</param>
        /// <returns>true, value exists, otherwhise false</returns>
        bool Exists(TEntity e);

        /// <summary>
        /// Get entity by primary key.
        /// </summary>
        /// <param name="id">target id</param>
        /// <returns>found entity, otherwise null value</returns>
        TEntity Get(ID id);

        /// <summary>
        /// Get entity by alternate key.
        /// </summary>
        /// <param name="uuid">target alternate key</param>
        /// <returns>found entity, otherwise null value</returns>
        TEntity GetByUuid(Guid uuid);

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="index">item index on persistence base, from 0</param>
        /// <param name="count">entity count by page list</param>
        /// <returns>found value, otherwhise empty list.</returns>
        List<TEntity> PagingIndex(int index, int count);

        /// <summary>
        /// List entities by paging.
        /// </summary>
        /// <param name="page">page index, from 0</param>
        /// <param name="limit">entity limit by page list, when -1 will use the max request limit default (<see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>).</param>
        /// <returns>found value, otherwhise empty list.</returns>
        List<TEntity> Paging(int page = 0, int limit = -1);

        /// <summary>
        /// List all values in database (limited to max request <see cref="IService{TEntity, ID}.REQUEST_LIST_LIMIT"/>, when more that it, use paging).
        /// </summary>
        /// <returns>list all values possible</returns>
        List<TEntity> List();
        #endregion

        #region [U]pdate
        /// <summary>
        /// Update entity on persistence base.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <returns>updated target entity</returns>
        TEntity Update(TEntity entity);
        #endregion

        #region [D]elete
        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <returns>amount of values removed</returns>
        int Delete(IEnumerable<Guid> uuid);

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <returns>amount of values removed</returns>
        int Delete(params Guid[] uuid);

        /// <summary>
        /// Attempt to delete values by uuid.
        /// </summary>
        /// <param name="uuid">uuids target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        bool Delete(Guid uuid);

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        bool Delete(IEnumerable<TEntity> entity);

        /// <summary>
        /// Attempt to delete entities.
        /// </summary>
        /// <param name="entity">entities target</param>
        /// <returns>true, removed value, otherwhise false.</returns>
        bool Delete(params TEntity[] entity);
        #endregion
    }
}
