using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Repositories
{
    /// <summary>
    /// Soft Delete Abstract Repository CRUD.
    ///<para>
    /// When model is ISoftDeleteModel, instead delete it, setup a value in Deleted property
    /// and this data is not more accessible by common methods in Repository. Buts keep it
    /// in database for historic track purporse.
    ///</para>
    /// </summary>
    /// <typeparam name="TContext">target DbContext</typeparam>
    /// <typeparam name="TEntity">target entity</typeparam>
    /// <typeparam name="ID">target entity id</typeparam>
    public abstract class RepositoryCrudSoftDelete<TContext, TEntity, ID> : CrudIdSoftDelete<TContext, TEntity, ID>
        where TEntity : class, ISoftDeleteModel<ID>, new()
        where TContext : ContextBase
    {
        /// <summary>
        /// Constructor for dbcontext and dbset.
        /// </summary>
        /// <param name="context">target dbcontext.</param>
        /// <param name="dbSet">target dbset.</param>
        public RepositoryCrudSoftDelete([NotNull] TContext context, [NotNull] DbSet<TEntity> dbSet)
            : base(context, dbSet) { }

        /// <summary>
        /// Constructor for dbcontext and
        /// autodiscovery dbset.
        /// </summary>
        /// <param name="context">target dbcontext.</param>
        public RepositoryCrudSoftDelete([NotNull] TContext context)
            : this(context, context?.GetOrSet<TEntity>()) { }

    }
}
