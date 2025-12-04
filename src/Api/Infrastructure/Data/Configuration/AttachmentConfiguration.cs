using Api.Features.Attachments.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configuration;

internal sealed class AttachmentConfiguration : AuditableConfiguration<Attachment>
{
    protected override void PostConfigure(
        EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.FileName)
            .IsRequired()
            .HasMaxLength(Attachment.MaxFileNameLength);

        builder.Property(t => t.ContentType)
            .IsRequired();

        builder.Property(t => t.FileSize)
            .IsRequired(false);

        builder.Property(t => t.BlobName)
            .IsRequired();

        builder.HasOne(a => a.Todo)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TodoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}