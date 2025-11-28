using Api.Features.Shared.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configuration;

internal abstract class AuditableConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class, IAuditable
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(Auditable.MaxCreatedByLength);

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(Auditable.MaxUpdatedByLength);
    }

    protected abstract void PostConfigure(EntityTypeBuilder<T> builder);
}