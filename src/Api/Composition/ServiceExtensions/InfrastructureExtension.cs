using Api.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ServiceDefaults;

namespace Api.Composition.ServiceExtensions;

public static class InfrastructureExtension
{
    extension(
        IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(
            WebApplicationBuilder builder)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();

            services.AddDbContext<ApplicationDbContext>((
                sp,
                opt) =>
            {
                opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                if(builder.Environment.IsDevelopment())
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }
                
                opt.UseNpgsql(builder.Configuration.GetConnectionString(AppHostConstants.Database));
            });

            return services;
        }
    }
}