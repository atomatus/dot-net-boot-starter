using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Entity service CRUD Async for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public interface IServiceCrudAsync<TEntity>
        : IService<TEntity>, ICrudAsync<TEntity> 
        where TEntity : IModel { }

    /// <summary>
    /// Entity service CRUD Async for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IServiceCrudAsync<TEntity, ID>
        : IServiceCrudAsync<TEntity>, IService<TEntity, ID>, ICrudAsync<TEntity, ID>
        where TEntity : IModel<ID> { }
}
