using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Repositories
{
    /// <summary>
    /// Entity Repository CRUD Async for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public interface IRepositoryCrudAsync<TEntity>
        : IRepository<TEntity>, ICrudAsync<TEntity>
        where TEntity : IModel
    { }

    /// <summary>
    /// Entity Repository CRUD Async for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IRepositoryCrudAsync<TEntity, ID>
        : IRepositoryCrudAsync<TEntity>, IRepository<TEntity, ID>, ICrudAsync<TEntity, ID>
        where TEntity : IModel<ID>
    { }
}
