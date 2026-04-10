using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Taskly.WebApi.Common.Infrastructure.Persistence;

namespace Taskly.IntegrationTests.Infrastructure.Composition.Mocks.Database;

public static class ApplicationDbContextMock
{
    internal static IServiceCollection AddMockDbContext(
        this IServiceCollection services,
        DbConnection connection)
    {
        services.RemoveAll<TasklyDbContext>();
        services.RemoveAll<DbContextOptions<TasklyDbContext>>();

        var interceptorDescriptors = services
            .Where(d =>
                typeof(SaveChangesInterceptor).IsAssignableFrom(d.ServiceType) ||
                (d.ImplementationType is not null &&
                 typeof(SaveChangesInterceptor).IsAssignableFrom(d.ImplementationType)) ||
                d.ImplementationInstance is SaveChangesInterceptor)
            .ToList();

        foreach (var d in interceptorDescriptors)
        {
            services.Remove(d);
        }

        services.AddDbContext<TasklyDbContext>(opt =>
        {
            opt.AddInterceptors(new ApplicationDbContextAuditableInterceptorMock());
            opt.EnableSensitiveDataLogging();
            opt.EnableDetailedErrors();
            opt.UseNpgsql(connection.ConnectionString);
        });

        return services;
    }
}