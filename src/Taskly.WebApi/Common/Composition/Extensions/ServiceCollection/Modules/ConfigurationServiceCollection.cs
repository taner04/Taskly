using Taskly.WebApi.Common.Composition.Options;
using Taskly.WebApi.Common.Composition.Serialization;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class ConfigurationServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyConfiguration(WebApplicationBuilder builder)
        {
            services.AddConfig<WebHookConfig>(builder);
            services.AddConfig<Auth0Config>(builder);
            services.AddConfig<EmailConfig>(builder);

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

        private IServiceCollection AddConfig<TOptions>(WebApplicationBuilder builder) where TOptions : class
        {
            var sectionName = typeof(TOptions).Name;

            var options = services.AddOptions<TOptions>()
                .Bind(builder.Configuration.GetSection(sectionName));

            if (!builder.Environment.IsEnvironment("Testing"))
            {
                options.ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            return services;
        }
    }
}