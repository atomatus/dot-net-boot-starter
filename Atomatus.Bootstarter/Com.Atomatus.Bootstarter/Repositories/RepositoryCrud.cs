using System.Diagnostics.CodeAnalysis;
using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter.Repositories
{
    /// <summary>
    /// Abstract Repository Crud.
    /// </summary>
    /// <typeparam name="TContext">target DbContext</typeparam>
    /// <typeparam name="TEntity">target entity</typeparam>
    /// <typeparam name="ID">target entity id</typeparam>
    public abstract partial class RepositoryCrud<TContext, TEntity, ID>
        : CrudId<TContext, TEntity, ID>,
        IRepositoryCrud<TEntity, ID>, IRepositoryCrudAsync<TEntity, ID>
        where TEntity : class, IModel<ID>, new()
        where TContext : ContextBase
    {
        /// <inheritdoc/>
        public RepositoryCrud([NotNull] TContext context, [NotNull] DbSet<TEntity> dbSet)
            : base(context, dbSet) { }

        /// <inheritdoc/>
        public RepositoryCrud([NotNull] TContext context) : base(context) { }

    }
}
