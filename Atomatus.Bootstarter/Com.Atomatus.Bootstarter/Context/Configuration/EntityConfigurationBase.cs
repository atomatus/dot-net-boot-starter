using Com.Atomatus.Bootstarter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Atomatus.Bootstarter.Context.Configuration
{
    public abstract class EntityConfigurationBase<TEntity> : 
        IEntityTypeConfiguration<TEntity>
        where TEntity : class, IModel, IModelAltenateKey
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasAlternateKey(e => e.Uuid);
            builder.Property(e => e.Uuid).ValueGeneratedOnAdd();
            this.OnConfigure(builder);
        }

        protected abstract void OnConfigure(EntityTypeBuilder<TEntity> builder);
    }
}
