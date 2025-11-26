using System.Diagnostics.CodeAnalysis;
using Api;
using Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddCustomizedProblemDetails();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-r2na1xjgqc87zkzh.us.auth0.com/";
    options.Audience = "https://taskly-api";
});

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