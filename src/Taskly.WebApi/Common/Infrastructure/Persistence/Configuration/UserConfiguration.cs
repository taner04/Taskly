using Taskly.WebApi.Features.Users.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

internal sealed class UserConfiguration : AuditableConfiguration<User>
{
    protected override void PostConfigure(
        EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired();

        builder.Property(u => u.Auth0Id)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
