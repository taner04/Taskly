using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Taskly.WebApi.Features.Users.Models;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

internal sealed class UserConfiguration : EntityConfiguration<User, UserId>
{
    protected override void PostConfigure(
        EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Email)
            .IsRequired();

        builder.Property(u => u.Auth0Id)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();
    }
}