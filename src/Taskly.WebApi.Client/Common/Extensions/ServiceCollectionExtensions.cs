using Microsoft.Extensions.DependencyInjection;
using Taskly.WebApi.Client.Abstractions;

namespace Taskly.WebApi.Client.Common.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApiHttpClient(Action<ApiHttpClientOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            services.AddSingleton<IApiHttpClient, ApiHttpClient>(_ =>
            {
                var apiHttpClientOptions = new ApiHttpClientOptions();
                options.Invoke(apiHttpClientOptions);

                return new ApiHttpClient(apiHttpClientOptions);
            });

            return services;
        }
    }
}
