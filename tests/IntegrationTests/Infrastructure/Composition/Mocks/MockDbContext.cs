using Api.Infrastructure.Data.Interceptors;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.Infrastructure.Composition.Mocks;

internal static class MockDbContext
{
    internal static IServiceCollection AddMockDbContext(
        this IServiceCollection services,
        DbConnection connection)
    {
        services.RemoveAll<ApplicationDbContext>();
        services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

        var interceptorDescriptors = services
            .Where(d => d.ServiceType == typeof(AuditableInterceptor)
                        || d.ImplementationType == typeof(AuditableInterceptor)
                        || d.ImplementationInstance is AuditableInterceptor)
            .ToList();

        foreach (var d in interceptorDescriptors)
        {
            services.Remove(d);
        }

        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.AddInterceptors(new MockAuditableInterceptor());
            opt.EnableSensitiveDataLogging();
            opt.EnableDetailedErrors();
            opt.UseNpgsql(connection.ConnectionString);
        });

        return services;
    }
}