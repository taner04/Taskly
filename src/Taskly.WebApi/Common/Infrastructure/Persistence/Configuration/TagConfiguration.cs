using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TagId = Taskly.WebApi.Features.Tags.Common.Models.TagId;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

internal sealed class TagConfiguration : EntityConfiguration<Tag, TagId>
{
    protected override void PostConfigure(
        EntityTypeBuilder<Tag> builder)
    {
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(Tag.MaxNameLength);

        builder.Property(t => t.UserId)
            .IsRequired();
    }
}