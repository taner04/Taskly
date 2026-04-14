using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Npgsql;
using Taskly.ServiceDefaults;
using Taskly.WebApi.IntegrationTests.Infrastructure.Composition.Mocks.Database;
using Taskly.WebApi.IntegrationTests.Infrastructure.Composition.Mocks.Jwt;

namespace Taskly.WebApi.IntegrationTests.Infrastructure;

public class WebApiFactory(string dbConnectionString, string azureBlobConnectionString)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureLogging(opt =>
        {
            opt.ClearProviders();
            opt.AddConsole();
        });

        builder.ConfigureServices(services =>
        {
            services.AddMockDbContext(dbConnectionString);
            services.AddMockJwtBearerOptions();
        });

        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", dbConnectionString);
        builder.UseSetting($"ConnectionStrings:{AppHostConstants.AzureBlobContainerName}", azureBlobConnectionString);
        builder.UseSetting("WebHookConfig:SecretKey", TestingFixture.WebHookSecret);
    }
}