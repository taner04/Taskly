using System.Data.Common;
using Api.Infrastructure.Data;
using Api.Infrastructure.Data.Interceptors;
using IntegrationTests.Common.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServiceDefaults;

namespace IntegrationTests.Common;

public class WebApiFactory(DbConnection connection) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(opt => { });
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            var interceptorDescriptors = services
                .Where(d => d.ServiceType == typeof(AuditableInterceptor)
                            || d.ImplementationType == typeof(AuditableInterceptor)
                            || (d.ImplementationInstance != null && d.ImplementationInstance.GetType() == typeof(AuditableInterceptor)))
                .ToList();

            foreach (var d in interceptorDescriptors)
            {
                services.Remove(d);
            }

            services.AddDbContext<ApplicationDbContext>((sp, opt) =>
            {
                opt.AddInterceptors(new MockAuditableInterceptor());
                opt.EnableSensitiveDataLogging();
                opt.EnableDetailedErrors();
                opt.UseNpgsql(connection);
            });
        });
        
        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", connection.ConnectionString);
    }
}