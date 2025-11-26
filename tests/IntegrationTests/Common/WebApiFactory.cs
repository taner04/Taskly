using System.Data.Common;
using Api.Infrastructure.Data;
using Api.Infrastructure.Data.Interceptors;
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
        
        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", connection.ConnectionString);
    }
}