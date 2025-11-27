using Api.Features.Shared.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configuration;

internal sealed class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.ToTable("Todos");

        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(Todo.MaxTitleLength);

        builder.Property(t => t.Description)
            .HasMaxLength(Todo.MaxDescriptionLength);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.IsCompleted)
            .IsRequired();

        builder.Property(t => t.UserId)
            .IsRequired()
            .HasMaxLength(Todo.MaxUserIdLength);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(Auditable.MaxCreatedByLength);

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(Auditable.MaxUpdatedByLength);
    }
}