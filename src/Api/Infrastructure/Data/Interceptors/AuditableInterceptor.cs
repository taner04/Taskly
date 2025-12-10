using Api.Features.Shared.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Api.Infrastructure.Data.Interceptors;

public sealed class AuditableInterceptor(CurrentUserService currentUserService) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            SetAuditableProperties(eventData.Context);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetAuditableProperties(
        DbContext context)
    {
        var auditableEntries = context.ChangeTracker
            .Entries<Auditable>()
            .ToList();

        var changeMadeBy = currentUserService.GetAuth0Id();

        foreach (var entry in auditableEntries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                {
                    entry.Entity.SetCreated(changeMadeBy);
                    break;
                }
                case EntityState.Modified:
                {
                    entry.Entity.SetUpdated(changeMadeBy);
                    break;
                }
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }
        }
    }
}