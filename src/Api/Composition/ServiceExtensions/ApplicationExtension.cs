using Api.Features.Attachments.Services;
using Azure.Storage.Blobs;
using ServiceDefaults;

namespace Api.Composition.ServiceExtensions;

public static class ApplicationExtension
{
    extension(
        IServiceCollection services)
    {
        public IServiceCollection AddApplication(
            IConfiguration configuration)
        {
            services.AddScoped<CurrentUserService>();

            services.AddSingleton(_ =>
            {
                var options = new BlobClientOptions();

                return new BlobServiceClient(
                    configuration.GetConnectionString(AppHostConstants.AzureBlobStorage),
                    options);
            });

            services.AddSingleton<AttachmentService>();
            return services;
        }
    }
}