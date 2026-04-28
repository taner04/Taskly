using Taskly.Shared.Extensions;
using Taskly.Shared.Options;
using Taskly.WebApi.Common.Composition.Options;
using Taskly.WebApi.Common.Composition.Serialization;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class ConfigurationServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyConfiguration(WebApplicationBuilder builder)
        {
            var isTestEnv = builder.Environment.IsEnvironment("Testing");
            var configuration = builder.Configuration;

            services.AddConfig<WebHookConfig>(configuration, isTestEnv);
            services.AddConfig<Auth0Config>(configuration, isTestEnv);
            services.AddConfig<EmailConfig>(configuration, isTestEnv);

            return services;
        }

        internal IServiceCollection AddCustomJsonConverter()
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new FlexibleEnumConverter<TodoPriority>());
                options.SerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
            });

            return services;
        }
    }
}