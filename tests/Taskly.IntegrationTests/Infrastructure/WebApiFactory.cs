using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Taskly.ServiceDefaults;
using Taskly.IntegrationTests.Infrastructure.Composition.Mocks.Database;
using Taskly.IntegrationTests.Infrastructure.Composition.Mocks.Jwt;
using Taskly.WebApi.Client.Common.Extensions;

namespace Taskly.IntegrationTests.Infrastructure;

public class WebApiFactory(DbConnection dbConnection, string azuriteConnectionString)
    : WebApplicationFactory<WebApi.Program>
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
            services.AddMockDbContext(dbConnection);
            services.AddMockJwtBearerOptions();
        });

        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", dbConnection.ConnectionString);
        builder.UseSetting($"ConnectionStrings:{AppHostConstants.AzureBlobStorage}", azuriteConnectionString);
    }
}
