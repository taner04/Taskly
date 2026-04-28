using Microsoft.Extensions.DependencyInjection;
using Taskly.WebApi.Client.Common;
using Taskly.WebApi.Client.Common.Refit.DelegatingHandlers;
using Taskly.WebApi.Client.Common.Services;

namespace Taskly.WebApi.Client.IoC;

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

            services.AddSingleton<IUserContext, UserContext>();
            services.AddSingleton<IAttachmentService, AttachmentService>();
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<ITodoService, TodoService>();
            services.AddSingleton<IUserService, UserService>();

            services.AddSingleton<IWebClientService, WebClientService>();

            return services;
        }
    }
}