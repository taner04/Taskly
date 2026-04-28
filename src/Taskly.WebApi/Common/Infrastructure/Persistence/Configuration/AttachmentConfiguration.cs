using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AttachmentId = Taskly.WebApi.Features.Attachments.Common.Models.AttachmentId;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

internal sealed class AttachmentConfiguration : EntityConfiguration<Attachment, AttachmentId>
{
    protected override void PostConfigure(
        EntityTypeBuilder<Attachment> builder)
    {
        builder.Property(t => t.FileName)
            .IsRequired()
            .HasMaxLength(Attachment.MaxFileNameLength);

        builder.Property(t => t.ContentType)
            .IsRequired();

        builder.Property(t => t.FileSize)
            .IsRequired();

        builder.Property(t => t.BlobName)
            .IsRequired();

        builder.HasOne(a => a.Todo)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TodoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}