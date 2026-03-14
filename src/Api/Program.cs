using System.Diagnostics.CodeAnalysis;
using Api.Common.Composition.Configs;
using Api.Common.Composition.Configs.OpenApi;
using Api.Common.Composition.ServiceExtensions;
using Api.Features.Attachments.Services;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi(OpenApiConfig.Config);

builder.Services.AddProblemDetails(ProblemDetailsConfig.Config);
builder.Services.AddHttpContextAccessor();

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
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapApiEndpoints();

using (var scope = app.Services.CreateScope())
{
    var attachmentService = scope.ServiceProvider.GetRequiredService<AttachmentService>();
    await attachmentService.InitializeAsync();
}

app.Run();


namespace Api
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Program; // For integration tests
}