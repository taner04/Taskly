using System.Data.Common;
using Api;
using Api.Infrastructure.Data.Interceptors;
using Azure.Storage.Blobs;
using IntegrationTests.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ServiceDefaults;

namespace IntegrationTests.Infrastructure;

public class WebApiFactory(DbConnection connection, string azuriteConnectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureLogging(opt =>
        {
            opt.ClearProviders();
            opt.AddConsole();
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            var interceptorDescriptors = services
                .Where(d => d.ServiceType == typeof(AuditableInterceptor)
                            || d.ImplementationType == typeof(AuditableInterceptor)
                            || d.ImplementationInstance is AuditableInterceptor)
                .ToList();

            foreach (var d in interceptorDescriptors)
            {
                services.Remove(d);
            }

            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.AddInterceptors(new MockAuditableInterceptor());
                opt.EnableSensitiveDataLogging();
                opt.EnableDetailedErrors();
                opt.UseNpgsql(connection.ConnectionString);
            });

            var existing = services.SingleOrDefault(s => s.ServiceType == typeof(BlobServiceClient));

            if (existing != null)
            {
                services.Remove(existing);
            }

            services.AddSingleton(new BlobServiceClient(azuriteConnectionString));
        });

        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", connection.ConnectionString);
    }
}