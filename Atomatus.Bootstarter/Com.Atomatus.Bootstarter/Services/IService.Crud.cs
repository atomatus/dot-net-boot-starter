using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Entity service CRUD for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public interface IServiceCrud<TEntity> :
        IService<TEntity>, ICrud<TEntity>
        where TEntity : IModel { }

    /// <summary>
    /// Entity service CRUD for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IServiceCrud<TEntity, ID> :
        IServiceCrud<TEntity>, IService<TEntity, ID>, ICrud<TEntity, ID>
        where TEntity : IModel<ID> { }
}
