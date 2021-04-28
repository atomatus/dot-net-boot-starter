using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter.Context.Ensures
{
    /// <summary>
    /// ContextBase definition calling ContextBase.Database.EnsureDeleted() on Dispose.
    /// </summary>
    public abstract class ContextBaseForEnsureDeletedOnDispose : ContextBaseForEnsure
    {
        /// <summary>
        /// Context base constructor receiving build parameters options,
        /// database schema name and defining whether attempt to load entity's
        /// configuration for each dbSet declared in context.
        /// </summary>
        /// <param name="options">
        /// The options for this context.
        /// </param>
        protected ContextBaseForEnsureDeletedOnDispose(DbContextOptions options) : base(options) { }

        /// <summary>
        ///  Releases the allocated resources for this context.
        /// </summary>
        /// <returns></returns>
        protected override void OnDispose()
        {
            if (this.IsEnsured())
            {
                this.Database.EnsureDeleted();
            }
        }
    }
}
