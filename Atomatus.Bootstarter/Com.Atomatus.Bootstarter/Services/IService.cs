using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IService { }

    /// <summary>
    /// Entity service interface
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IService<TEntity> : IService
        where TEntity : IModel { }

    /// <summary>
    /// Entity service interface generic.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IService<TEntity, ID> : IService<TEntity>
        where TEntity : IModel<ID> 
    {
        /// <summary>
        /// Request list limit values (300).
        /// </summary>
        const int REQUEST_LIST_LIMIT = 300;
    }
}
