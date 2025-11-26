using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using ServiceDefaults;

namespace IntegrationTests.Common;

public class WebApiFactory(DbConnection connection) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(opt => { });

        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", connection.ConnectionString);
    }
}