using Microsoft.Extensions.DependencyInjection;
using Taskly.WebApi.Client.Common;
using Taskly.WebApi.Client.Common.DelegatingHandlers;
using Taskly.WebApi.Client.Common.Services;
using Taskly.WebApi.Client.Features.Attachments.Services;
using Taskly.WebApi.Client.Features.Tags.Services;
using Taskly.WebApi.Client.Features.Todos.Services;
using Taskly.WebApi.Client.Features.Users.Services;

namespace Taskly.WebApi.Client;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddWebApiClient(Action<WebClientOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);
            
            var optionsInstance = new WebClientOptions();
            options(optionsInstance);
            
            services.AddSingleton<WebClientOptions>(_ => optionsInstance);
            
            services.AddSingleton<BearerDelegatingHandler>();
            services.AddRefitClient<IRefitWebApiClient>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = optionsInstance.BaseAddress; 
                    c.Timeout = optionsInstance.Timeout;
                })
                .AddHttpMessageHandler<BearerDelegatingHandler>();

            services.AddSingleton<BearerTokeStore>();
            services.AddSingleton<AttachmentService>();
            services.AddSingleton<TagService>();
            services.AddSingleton<TodoService>();
            services.AddSingleton<UserService>();

            services.AddSingleton<WebClientService>();

            return services;
        }
    }
}