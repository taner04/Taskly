using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Taskly.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddConfig<TOptions>(IConfiguration configuration, bool isTestingEnv)
            where TOptions : class
        {
            var sectionName = typeof(TOptions).Name;

            var options = services.AddOptions<TOptions>()
                .Bind(configuration.GetSection(sectionName));

            if (!isTestingEnv)
            {
                options.ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            return services;
        }
    }
}