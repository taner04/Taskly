using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Taskly.Shared.Extensions;
using Taskly.WebApi.Common.Composition.Options;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class AuthenticationServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyAuthentication(
            IConfiguration configuration)
        {
            var auth0Config = configuration.GetOptions<Auth0Config>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{auth0Config.Domain}";
                options.Audience = auth0Config.Audience;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = auth0Config.Audience,
                    ValidIssuer = $"https://{auth0Config.Domain}/",
                    RoleClaimType = CurrentUserService.RoleClaim,
                    NameClaimType = CurrentUserService.SubClaim
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(Security.Policies.Admin,
                    policy => policy.RequireClaim("permissions", "admin:write", "admin:read"))
                .AddPolicy(Security.Policies.User,
                    policy => policy.RequireClaim("permissions", "user:write", "user:read"));

            return services;
        }
    }
}