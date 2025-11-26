using Api.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IntegrationTests.Common.Database;

public sealed class MockAuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            SetAuditableProperties(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetAuditableProperties(DbContext context)
    {
        var auditableEntities = context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAuditable)
            .ToList();

        const string changeMadeBy = "system";

        foreach (var entry in auditableEntities)
        {
            if (entry.Entity is not IAuditable auditable)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                {
                    auditable.SetCreated(changeMadeBy);
                    break;
                }
                case EntityState.Modified:
                {
                    auditable.SetUpdated(changeMadeBy);
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