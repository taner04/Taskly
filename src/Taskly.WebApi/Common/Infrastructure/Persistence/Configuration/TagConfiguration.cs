using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

internal sealed class TagConfiguration : AuditableConfiguration<Tag>
{
    protected override void PostConfigure(
        EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(Tag.MaxNameLength);

        builder.Property(t => t.UserId)
            .IsRequired();
    }
}
