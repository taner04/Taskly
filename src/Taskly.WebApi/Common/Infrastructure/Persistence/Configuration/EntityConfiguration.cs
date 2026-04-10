using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Taskly.WebApi.Common.Shared.Models;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

internal abstract class EntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TId>
    where TId : struct
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // IEntity configuration
        builder.ToTable($"{typeof(TEntity).Name}s");
        builder.HasKey(t => t.Id);

        // IAuditable configuration
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(Auditable.MaxCreatedByLength);

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(Auditable.MaxUpdatedByLength);

        PostConfigure(builder);
    }

    protected abstract void PostConfigure(
        EntityTypeBuilder<TEntity> builder);
}