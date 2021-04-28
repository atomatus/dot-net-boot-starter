using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter.Context.Ensures
{
    /// <summary>
    /// ContextBase definition calling ContextBase.Database.EnsureCreated()
    /// in constructor and calling ContextBase.Database.EnsureDeleted() on Dispose.
    /// </summary>
    public abstract class ContextBaseForEnsureCreationAndDeletedOnDispose : ContextBaseForEnsureDeletedOnDispose
    {
        /// <summary>
        /// Context base constructor receiving build parameters options,
        /// database schema name and defining whether attempt to load entity's
        /// configuration for each dbSet declared in context.
        /// </summary>
        /// <param name="options">
        /// The options for this context.
        /// </param>
        protected ContextBaseForEnsureCreationAndDeletedOnDispose(DbContextOptions options) : base(options)
        {
            if (this.CanEnsure())
            {
                this.Database.EnsureCreated();
            }
        }

    }
}
