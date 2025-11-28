using System.Diagnostics.CodeAnalysis;
using Api;
using Api.Composition.OpenApiDocumentTransformers;
using Api.Composition.ServiceExtensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi(options => { options.AddDocumentTransformer<BearerDocumentTransformer>(); });

builder.Services.AddCustomizedProblemDetails();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
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

app.Run();


namespace Api
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Program; // For integration tests
}