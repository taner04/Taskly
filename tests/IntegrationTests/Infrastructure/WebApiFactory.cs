using Api.Client;
using IntegrationTests.Infrastructure.Composition.Mocks;
using IntegrationTests.Infrastructure.Composition.ServiceExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Refit;
using ServiceDefaults;

namespace IntegrationTests.Infrastructure;

public class WebApiFactory(DbConnection dbConnection, string azuriteConnectionString)
    : WebApplicationFactory<Api.Program>
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
            services.AddRefitClient<IApiClient>();
        });

        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", dbConnection.ConnectionString);
        builder.UseSetting($"ConnectionStrings:{AppHostConstants.AzureBlobStorage}", azuriteConnectionString);
    }
}