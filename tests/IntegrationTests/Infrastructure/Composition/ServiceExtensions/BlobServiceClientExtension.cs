namespace IntegrationTests.Infrastructure.Composition.ServiceExtensions;

internal static class BlobServiceClientExtension
{
    internal static IServiceCollection AddMockBlobServiceClient(this IServiceCollection services,
        string connectionString)
    {
        var existing = services.SingleOrDefault(s => s.ServiceType == typeof(BlobServiceClient));

        if (existing != null) services.Remove(existing);

        services.AddSingleton(new BlobServiceClient(connectionString));

        return services;
    }
}