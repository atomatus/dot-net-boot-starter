using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// <para>
    /// Allows configuration for an entity type to be factored into a separate class,
    /// rather than in-line in <see cref="ContextBase.OnModelCreating(ModelBuilder)"/>.
    /// </para>
    /// Implement this abstract class, applying configuration for the entity in the <see cref="Configure(EntityTypeBuilder{TEntity})"/><br/>
    /// method, and then apply the configuration to the model using <see cref="ModelBuilder.ApplyConfiguration{TEntity}(IEntityTypeConfiguration{TEntity})"/><br/>
    /// in <see cref="ContextBase.OnModelCreating(ModelBuilder)"/>.
    /// <para>
    /// By default the <see cref="Configure(EntityTypeBuilder{TEntity})"/> set the <see cref="IModel{ID}.Id"/> as primary key and<br/>
    /// if <typeparamref name="TEntity"/> is <see cref="IModelAltenateKey"/>,
    /// set the <see cref="IModelAltenateKey.Uuid"/> as alternate key generating value on add.
    /// </para>
    /// <para>
    /// <i>
    /// Remember: the properties <see cref="IModel{ID}.Id"/>, 
    /// <see cref="IModelAltenateKey.Uuid"/> are already set by default.<br/>
    /// Configure only your own properties defined.
    /// </i>
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
    public abstract class EntityConfigurationBase<TEntity, ID> : 
        IEntityTypeConfiguration<TEntity>
        where TEntity : class, IModel<ID>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);

            if(typeof(IModelAltenateKey).IsAssignableFrom(typeof(TEntity)))
            {
                builder.HasAlternateKey(nameof(IModelAltenateKey.Uuid));
                builder.Property(nameof(IModelAltenateKey.Uuid)).ValueGeneratedOnAdd();
            }
            
            this.OnConfigure(builder);
        }

        /// <summary>
        /// Configures the entity of type TEntity.
        /// <para>
        /// <i>
        /// Remember: the properties <see cref="IModel{ID}.Id"/>, 
        /// <see cref="IModelAltenateKey.Uuid"/> are already set by default.<br/>
        /// Configure only your own properties defined.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        protected abstract void OnConfigure(EntityTypeBuilder<TEntity> builder);
    }
}
