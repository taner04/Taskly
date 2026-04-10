using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Taskly.ServiceDefaults;
using Taskly.WebApi.Common.Composition.Configs;
using Taskly.WebApi.Common.Composition.Configs.OpenApi;
using Taskly.WebApi.Common.Composition.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCustomJsonConverter();
builder.Services.AddOpenApi(OpenApiConfig.Config);

builder.Services.AddProblemDetails(ProblemDetailsConfig.Config);
builder.Services.AddHttpContextAccessor();

builder.Services.AddRateLimiting();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder);
builder.Services.AddImmediate();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalar();
    app.AddHangfireDashboard();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseHttpsRedirection();
app.MapTasklyWebApiEndpoints();

await app.InitializeBlobStorage();

app.Run();

namespace Taskly.WebApi
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Program; // For integration tests
}