using Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddInfrastructure(WebApplicationBuilder builder)
        {
            services
                .AddCustomJsonConverter()
                .AddTasklyConfiguration(builder)
                .AddTasklyAuthentication(builder.Configuration)
                .AddTasklyRateLimiting()
                .AddTasklyWebApiBehaviors()
                .AddTasklyWebApiHandlers()
                .AddTasklyDbContext(builder)
                .AddTasklyAzureBlobClient(builder.Configuration)
                .AddTasklyServices()
                .AddTasklyAttachmentEventStrategies()
                .AddTasklyHangfire(builder);

            return services;
        }
    }
}