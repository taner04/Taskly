using Api.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IntegrationTests.Infrastructure.Data;

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

    private static void SetAuditableProperties(
        DbContext context)
    {
        var auditableEntries = context.ChangeTracker
            .Entries<IAuditable>()
            .ToList();

        foreach (var entity in auditableEntries.Select(entry => entry.Entity))
        {
            entity.SetCreated("system");
            entity.SetUpdated("system");
        }
    }
}