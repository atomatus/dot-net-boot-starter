using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Abstract Service Crud.
    /// </summary>
    /// <typeparam name="TContext">target DbContext</typeparam>
    /// <typeparam name="TEntity">target entity</typeparam>
    /// <typeparam name="ID">target entity id</typeparam>
    public abstract partial class ServiceCrud<TContext, TEntity, ID>
        : CrudId<TContext, TEntity, ID>,
        IServiceCrud<TEntity, ID>, IServiceCrudAsync<TEntity, ID>
        where TEntity : class, IModel<ID>, new()
        where TContext : ContextBase
    {
        /// <inheritdoc/>
        public ServiceCrud([NotNull] TContext context, [NotNull] DbSet<TEntity> dbSet)
            : base(context, dbSet) { }

        /// <inheritdoc/>
        public ServiceCrud([NotNull] TContext context) : base(context) { }

    }
}
