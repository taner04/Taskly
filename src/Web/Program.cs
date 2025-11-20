using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Radzen;
using ServiceDefaults;
using Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCascadingAuthenticationState();

builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0 Domain is not configured.");
        options.ClientId = builder.Configuration["Auth0:ClientId"] ?? throw new InvalidOperationException("Auth0 ClientId is not configured.");
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"] ?? throw new InvalidOperationException("Auth0 ClientSecret is not configured.");

        options.OpenIdConnectEvents = new OpenIdConnectEvents()
        {
            OnTokenValidated = async context =>
            {
                //TODO: Save JWT token in a cookie or session storage for later use
                await ValueTask.CompletedTask;
            }
        };
    })
    .WithAccessToken(options =>
    {
        options.Audience = builder.Configuration["Auth0:Audience"] ?? throw new InvalidOperationException("Auth0 Audience is not configured.");
    });

builder.Services.AddHttpClient();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddRadzenComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapWebEndpoints();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
