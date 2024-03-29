﻿using Microsoft.EntityFrameworkCore;

namespace Com.Atomatus.Bootstarter.Context.Ensures
{
    /// <summary>
    /// ContextBase definition calling ContextBase.Database.EnsureCreated()
    /// in constructor.
    /// </summary>
    public abstract class ContextBaseForEnsureCreation : ContextBaseForEnsure
    {
        /// <summary>
        /// Context base constructor receiving build parameters options,
        /// database schema name and defining whether attempt to load entity's
        /// configuration for each dbSet declared in context.
        /// </summary>
        /// <param name="options">
        /// The options for this context.
        /// </param>
        protected ContextBaseForEnsureCreation(DbContextOptions options) : base(options) 
        {
            if (this.CanEnsure())
            {
                this.Database.EnsureCreated();
            }
        }
    }
}
