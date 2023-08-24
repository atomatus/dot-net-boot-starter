using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Repositories
{
    /// <summary>
    /// Entity Repository CRUD for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public interface IRepositoryCrud<TEntity>
        : IRepository<TEntity>, ICrud<TEntity>
        where TEntity : IModel
    { }

    /// <summary>
    /// Entity Repository CRUD for DbContext.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IRepositoryCrud<TEntity, ID>
        : IRepositoryCrud<TEntity>, IRepository<TEntity, ID>, ICrud<TEntity, ID>
        where TEntity : IModel<ID>
    { }
}
