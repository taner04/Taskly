using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace IntegrationTests.Infrastructure.Mocks.Jwt;

public static class JwtBearerOptionsMock
{
    internal static IServiceCollection AddMockJwtBearerOptions(
        this IServiceCollection services)
    {
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters.ValidIssuer = JwtTokenMock.Issuer;
            options.TokenValidationParameters.ValidAudience = JwtTokenMock.Audience;
            options.TokenValidationParameters.IssuerSigningKey = JwtTokenMock.SecurityKey;
        });

        return services;
    }
}