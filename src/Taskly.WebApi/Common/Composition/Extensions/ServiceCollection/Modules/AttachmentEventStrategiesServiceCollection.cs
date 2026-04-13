using Taskly.WebApi.Features.Attachments.WebHooks;
using Taskly.WebApi.Features.Attachments.WebHooks.EventStrategies;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class AttachmentEventStrategiesServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyAttachmentEventStrategies()
        {
            services.AddScoped<IAttachmentUploadEventStartegie, AttachmentUploadCompletedEventStrategy>();
            services.AddScoped<IAttachmentUploadEventStartegie, AttachmentUploadFailedEventStrategy>();
            services
                .AddScoped<IAttachmentUploadEventStartegie<AttachmentEventData.Completed>,
                    AttachmentUploadCompletedEventStrategy>();
            services
                .AddScoped<IAttachmentUploadEventStartegie<AttachmentEventData.Failed>,
                    AttachmentUploadFailedEventStrategy>();
            services.AddScoped<AttachmentEventDispatcher>();

            return services;
        }
    }
}