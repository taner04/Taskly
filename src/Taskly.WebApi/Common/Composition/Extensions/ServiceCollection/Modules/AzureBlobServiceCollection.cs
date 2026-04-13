using Azure.Storage.Blobs;
using Taskly.ServiceDefaults;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class AzureBlobServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyAzureBlobClient(ConfigurationManager configuration)
        {
            services.AddSingleton(_ =>
            {
                var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2024_11_04);

                return new BlobServiceClient(
                    configuration.GetConnectionString(AppHostConstants.AzureBlobStorage),
                    options);
            });

            return services;
        }
    }
}