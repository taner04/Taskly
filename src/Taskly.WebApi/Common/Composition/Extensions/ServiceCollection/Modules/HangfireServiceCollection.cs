using Hangfire;
using Hangfire.PostgreSql;
using Taskly.ServiceDefaults;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class HangfireServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyHangfire(WebApplicationBuilder builder)
        {
            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(c =>
                    c.UseNpgsqlConnection(builder.Configuration.GetConnectionString(AppHostConstants.Database))));

            services.AddHangfireServer(options => { options.SchedulePollingInterval = TimeSpan.FromSeconds(1); });

            return services;
        }
    }
}