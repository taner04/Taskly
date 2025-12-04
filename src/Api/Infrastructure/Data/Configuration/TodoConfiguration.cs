using Api.Features.Todos.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configuration;

internal sealed class TodoConfiguration : AuditableConfiguration<Todo>
{
    protected override void PostConfigure(
        EntityTypeBuilder<Todo> builder)
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
            .IsRequired();

        builder
            .HasMany(t => t.Tags)
            .WithMany(t => t.Todos)
            .UsingEntity(j => { j.ToTable("TodoTags"); });

        builder
            .HasMany(t => t.Attachments)
            .WithOne(a => a.Todo)
            .HasForeignKey(a => a.TodoId);
    }
}