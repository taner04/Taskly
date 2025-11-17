using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Taskly.Api.Behaviors;
using Taskly.Api.Features.Shared;
using Taskly.Api.Infrastructure.Data;
using Taskly.Api.Infrastructure.Data.Interceptors;

namespace Taskly.Api;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            
            services.AddMediator(opt =>
            {
                opt.ServiceLifetime = ServiceLifetime.Scoped;
                opt.PipelineBehaviors =
                [
                    typeof(LoggingBehaviour<,>),
                    typeof(ValidationBehaviour<,>),
                    typeof(CurrentUserEnricherBehaviour<,>)
                ];
            });

            services.AddScoped<CurrentUserService>();

            return services;
        }

        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, opt) =>
            {
                var interceptors = sp.GetServices<ISaveChangesInterceptor>().ToList();
                interceptors.ForEach(interceptor => { opt.AddInterceptors(interceptor); });

                opt.EnableSensitiveDataLogging();
                opt.EnableDetailedErrors();
                opt.UseNpgsql(configuration.GetConnectionString(Constants.Database));
            });

            return services;
        }
    }
}