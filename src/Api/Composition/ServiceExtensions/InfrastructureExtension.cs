using Api.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ServiceDefaults;

namespace Api.Composition.ServiceExtensions;

public static class InfrastructureExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(
            IConfiguration configuration)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();

            services.AddDbContext<ApplicationDbContext>((
                sp,
                opt) =>
            {
                var interceptors = sp.GetServices<ISaveChangesInterceptor>().ToList();
                interceptors.ForEach(interceptor => { opt.AddInterceptors(interceptor); });

                opt.EnableSensitiveDataLogging();
                opt.EnableDetailedErrors();
                opt.UseNpgsql(configuration.GetConnectionString(AppHostConstants.Database));
            });

            return services;
        }
    }
}