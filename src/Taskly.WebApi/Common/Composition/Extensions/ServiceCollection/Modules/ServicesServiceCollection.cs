using Taskly.WebApi.Common.Infrastructure.Services.Emails;
using Taskly.WebApi.Common.Shared.Pagination;
using AttachmentBlobContainerService =
    Taskly.WebApi.Features.Attachments.Common.Services.AttachmentBlobContainerService;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class ServicesServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyServices()
        {
            services.AddScoped<CurrentUserService>();
            services.AddScoped<EmailService>();
            services.AddScoped<PaginationService>();
            services.AddSingleton<AttachmentBlobContainerService>();

            return services;
        }
    }
}