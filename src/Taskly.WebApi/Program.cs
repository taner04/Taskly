using System.Diagnostics.CodeAnalysis;
using Taskly.ServiceDefaults;
using Taskly.WebApi.Common.Composition.Configs;
using Taskly.WebApi.Common.Composition.Configs.OpenApi;
using Taskly.WebApi.Common.Composition.Extensions;
using Taskly.WebApi.Common.Composition.Extensions.ServiceCollection;

var builder = WebApplication.CreateBuilder(args);

_ = builder.AddServiceDefaults();
_ = builder.Services.AddOpenApi(OpenApiConfig.Config);
_ = builder.Services.AddProblemDetails(ProblemDetailsConfig.Config);
_ = builder.Services.AddHttpContextAccessor();
_ = builder.Services.AddInfrastructure(builder);

var app = builder.Build();

_ = app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
    _ = app.MapScalar();
    _ = app.AddHangfireDashboard();
}

_ = app.UseExceptionHandler();

_ = app.UseAuthentication();
_ = app.UseAuthorization();

_ = app.UseRateLimiter();

_ = app.UseHttpsRedirection();
_ = app.MapTasklyWebApiEndpoints();

_ = await app.InitializeBlobStorage();

app.Run();

namespace Taskly.WebApi
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Program; // For integration tests
}