using IntegrationTests.Infrastructure.Composition.Mocks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace IntegrationTests.Infrastructure.Composition.ServiceExtensions;

internal static class MockJwtBearerOptions
{
    internal static IServiceCollection AddMockJwtBearerOptions(
        this IServiceCollection services)
    {
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var config = new OpenIdConnectConfiguration
            {
                Issuer = MockJwtTokens.Issuer
            };

            config.SigningKeys.Add(MockJwtTokens.SecurityKey);
            options.Configuration = config;
        });

        return services;
    }
}
