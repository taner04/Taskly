using Microsoft.EntityFrameworkCore.Diagnostics;
using Taskly.ServiceDefaults;
using Taskly.WebApi.Common.Infrastructure.Persistence.Interceptors;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class TasklyDbContextServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyDbContext(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString(AppHostConstants.Database);
            ArgumentNullException.ThrowIfNull(connectionString);


            services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();
            services.AddDbContext<TasklyDbContext>((
                sp,
                opt) =>
            {
                opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                if (builder.Environment.IsDevelopment())
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }

                opt.UseNpgsql(connectionString);
            });

            return services;
        }
    }
}