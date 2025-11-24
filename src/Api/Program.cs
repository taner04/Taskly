using System.Diagnostics.CodeAnalysis;
using Api;
using Api.Extensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddCustomizedProblemDetails();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
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


[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Program; // For integration tests