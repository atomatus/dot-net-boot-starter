using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Repositories
{
    /// <summary>
    /// Repository interface.
    /// </summary>
	public interface IRepository { }

    /// <summary>
    /// Entity repository interface.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IRepository
        where TEntity : IModel { }

    /// <summary>
    /// Entity repository interface generic.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public interface IRepository<TEntity, ID> : IRepository<TEntity>
        where TEntity : IModel<ID> { }
}
