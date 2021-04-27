using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// <para>
    /// A DbContext instance represents a session with the database and can be used to
    /// query and save instances of your entities. DbContext is a combination of the
    /// Unit Of Work and Repository patterns.
    /// </para>
    /// <para>
    /// Database Context to be used for <see cref="IModel{ID}"/>, <see cref="AuditableModel{ID}"/>
    /// entities.
    /// </para>
    /// See more in <see cref="DbContext"/>
    /// </summary>
    public abstract partial class ContextBase : DbContext
    {
        /// <summary>
        /// Database schema name.
        /// </summary>
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
        protected ContextBase(DbContextOptions options, string schemaName, bool loadEntityConfigurationByEachDbSet) : base(options)
        {
            this.SchemaName = schemaName;
            this.loadEntityConfigurationByEachDbSet = loadEntityConfigurationByEachDbSet;
            this.dbSetDic = new ConcurrentDictionary<Type, object>();
            this.Database.EnsureCreated();
        }

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
        protected ContextBase(DbContextOptions options, string schemaName) : this(options, schemaName, true) { }

        /// <summary>
        /// Context base constructor receiving build parameters options,
        /// database schema name and defining whether attempt to load entity's
        /// configuration for each dbSet declared in context.
        /// </summary>
        /// <param name="options">
        /// The options for this context.
        /// </param>
        protected ContextBase(DbContextOptions options) : this(options, null, true) { }

        /// <summary>
        /// <para>
        /// Override this method to further configure the model that was discovered by convention
        /// from the entity types exposed in Microsoft.EntityFrameworkCore.DbSet`1 properties
        /// on your derived context. The resulting model may be cached and re-used for subsequent
        /// instances of your derived context.
        /// </para>
        /// <para>
        /// Whether declared <see cref="DbSet{TEntity}"/> how properties for your entity,
        /// is not necessary request <see cref="ModelBuilder.ApplyConfiguration"/> 
        /// because this method will try to load it for autodiscovery (in same assembly).
        /// But, if not set an entity <see cref="DbSet{TEntity}"/> how propertiy, you have
        /// to specify your <see cref="IEntityTypeConfiguration{TEntity}"/> 
        /// (when your are using <see cref="IModel{ID}"/> try to use <see cref="EntityTypeConfigurationBase{TEntity, ID}"/>) 
        /// for each not declared entity dbSet.
        /// </para>
        /// </summary>
        /// <param name="modelBuilder">
        /// The builder being used to construct the model for this context. Databases (and
        /// other extensions) typically define extension methods on this object that allow
        /// you to configure aspects of the model that are specific to a given database.
        /// </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SchemaName);            
            base.OnModelCreating(modelBuilder);            
            this.AttemptLoadEntityConfigurationsDeclaredToDbSetDeclared(modelBuilder);
        }

        /// <summary>
        /// This method will be fired previous save any entity changes in database.
        /// </summary>
        /// <param name="entries">target entities to change</param>
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
                            if(model is IModelAltenateKey) entry.Property(nameof(IModelAltenateKey.Uuid)).IsModified = false;
                            goto case EntityState.Modified;
                        case EntityState.Modified when model is IModelAltenateKey:
                            entry.Property(nameof(IModelAltenateKey.Uuid)).IsModified = false;
                            goto case EntityState.Modified;
                        case EntityState.Modified:
                            entry.Property(nameof(IModel<int>.Id)).IsModified = false;
                            break;
                    }
                }
            }
        }

        #region SaveChanges
        /// <summary>
        /// <para>
        /// Saves all changes made in this context to the database.<br/>
        /// This method will automatically call <see cref="ChangeTracker.DetectChanges"/><br/>
        /// to discover any changes to entity instances before saving to the underlying database.<br/>
        /// <i>This can be disabled via <see cref="ChangeTracker.AutoDetectChangesEnabled"/>.</i>
        /// </para>
        /// <para>
        /// Before save changes will be executed the method <see cref="OnPrevSaveChanges(IEnumerable{EntityEntry})"/>
        /// with all detected tracked changes.
        /// </para>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">
        /// Indicates whether <see cref="ChangeTracker.AcceptAllChanges"/>
        /// is called after the changes have been sent successfully to the database.
        /// </param>
        /// <returns>
        /// The number of state entries written to the database.
        /// </returns>
        /// <exception cref="DbUpdateException">
        /// An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        /// A concurrency violation is encountered while saving to the database.<br/>
        /// A concurrency violation occurs when an unexpected number of rows are affected during save.<br/>
        /// This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public sealed override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.OnPrevSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// <para>
        /// Saves all changes made in this context to the database.<br/>
        /// This method will automatically call <see cref="ChangeTracker.DetectChanges"/><br/>
        /// to discover any changes to entity instances before saving to the underlying database.<br/>
        /// <i>This can be disabled via <see cref="ChangeTracker.AutoDetectChangesEnabled"/>.</i>
        /// </para>
        /// <para>
        /// Multiple active operations on the same context instance are not supported.<br/>
        /// Use 'await' to ensure that any asynchronous operations have completed before calling
        /// another method on this context.
        /// </para>
        /// <para>
        /// Before save changes will be executed the method <see cref="OnPrevSaveChanges(IEnumerable{EntityEntry})"/>
        /// with all detected tracked changes.
        /// </para>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">
        /// Indicates whether <see cref="ChangeTracker.AcceptAllChanges"/>
        /// is called after the changes have been sent successfully to the database.
        /// </param>
        /// <param name="cancellationToken">
        /// A System.Threading.CancellationToken to observe while waiting 
        /// for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous save operation.The task result contains
        /// the number of state entries written to the database.
        /// </returns>
        /// <exception cref="DbUpdateException">
        /// An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        /// A concurrency violation is encountered while saving to the database.<br/>
        /// A concurrency violation occurs when an unexpected number of rows are affected during save.<br/>
        /// This is usual
        /// </exception>
        public sealed override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(this.OnPrevSaveChanges)
                .ContinueWith(r => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken), cancellationToken)
                .Unwrap();
        }
        #endregion

        /// <summary>
        /// Invoke this method to detach all entities tracked by current dbContext.
        /// </summary>
        protected internal void DetachAllEntities()
        {
            if (this.ChangeTracker?.AutoDetectChangesEnabled ?? false)
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
