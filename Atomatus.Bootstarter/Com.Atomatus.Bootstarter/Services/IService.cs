using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Services
{
    public interface IService { }

    public interface IService<TEntity> : IService
        where TEntity : IModel { }

    public interface IService<TEntity, ID> : IService<TEntity>
        where TEntity : IModel<ID> { }
}
