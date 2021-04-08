using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Model.Auditable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Context
{
    public abstract partial class ContextBase : DbContext
    {
        protected readonly string SchemaName;
        private readonly bool loadEntityConfigurationByEachDbSet;

        /// <summary>
        /// Context base constructor receiving build parameters options,
        /// database schema name and defining whether attempt to load entity's
        /// configuration for each dbSet declared in context.
        /// </summary>
        /// <param name="options">
        /// The options for this context.
        /// </param>
        /// <param name="schemaName">
        /// Context schema name
        /// </param>
        /// <param name="loadEntityConfigurationByEachDbSet">
        /// When true, try find and load <see cref="IEntityTypeConfiguration{TEntity}"/>
        /// implementation for each dbset declared in current context.
        /// </param>
        protected ContextBase(DbContextOptions options, 
            string schemaName = null, 
            bool loadEntityConfigurationByEachDbSet = true) : base(options)
        {
            this.SchemaName = schemaName;
            this.loadEntityConfigurationByEachDbSet = loadEntityConfigurationByEachDbSet;
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SchemaName);
            base.OnModelCreating(modelBuilder);
            this.AttemptToLoadEntityConfigurationFromDbSetDeclared(modelBuilder);
        }

        protected virtual void OnPrevSaveChanges(IEnumerable<EntityEntry> entries) { }

        private void OnPrevSaveChanges()
        {
            this.OnAuditEntity(ChangeTracker
               .Entries()
               .Where(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified));

            this.OnPrevSaveChanges(ChangeTracker.Entries());
        }

        private void OnAuditEntity(IEnumerable<EntityEntry> entries)
        {
            DateTime now = DateTime.Now;

            foreach (EntityEntry entry in entries)
            {
                if (entry.Entity is IModel model)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added when model is IAudit audit:
                            audit.Created = now;
                            audit.Updated = null;
                            break;
                        case EntityState.Modified when model is IAudit audit:
                            audit.Updated = now;
                            entry.Property(nameof(IAudit.Created)).IsModified = false;
                            goto case EntityState.Modified;
                        case EntityState.Modified:
                            entry.Property(nameof(IModel.Id)).IsModified = false;
                            entry.Property(nameof(IModelAltenateKey.Uuid)).IsModified = false;
                            break;
                        default:
                            entry.Property(nameof(IAudit.Created)).IsModified = false;
                            entry.Property(nameof(IAudit.Updated)).IsModified = false;
                            break;
                    }
                }
            }
        }

        #region SaveChanges
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.OnPrevSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(this.OnPrevSaveChanges)
                .ContinueWith(r => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken), cancellationToken)
                .Unwrap();
        }
        #endregion

        protected internal void DetachAllEntities()
        {
            if (this.ChangeTracker.AutoDetectChangesEnabled)
            {
                this.ChangeTracker
                    .Entries()
                    .Where(e => e.State != EntityState.Detached)
                    .AsParallel()
                    .ForAll(e => e.State = EntityState.Detached);
            }
        }
    }
}
