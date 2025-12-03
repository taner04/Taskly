using Api.Features.Attachments.Services;
using Api.Features.Users;
using Azure.Storage.Blobs;

namespace Api.Composition.ServiceExtensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<CurrentUserService>();

        services.AddSingleton(_ =>
        {
            // IMPORTANT: set service version in constructor
            var options = new BlobClientOptions();

            return new BlobServiceClient(
                configuration.GetConnectionString("AzureBlobStorage"),
                options);
        });

        services.AddSingleton<AttachmentService>();
        return services;
    }
}