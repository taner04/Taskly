using Api.Features.Tags.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configuration;

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