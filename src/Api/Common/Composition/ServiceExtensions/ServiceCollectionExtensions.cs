using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Persistence.Interceptors;
using Api.Features.Attachments.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using ServiceDefaults;

namespace Api.Common.Composition.ServiceExtensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication(
            IConfiguration configuration)
        {
            services.AddScoped<CurrentUserService>();

            services.AddSingleton(_ =>
            {
                var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2024_11_04);

                return new BlobServiceClient(
                    configuration.GetConnectionString(AppHostConstants.AzureBlobStorage),
                    options);
            });

            services.AddSingleton<AttachmentService>();
            return services;
        }

        public IServiceCollection AddAuthenticationAndAuthorization(
            IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{configuration["Auth0:Domain"]}";
                options.Audience = configuration["Auth0:Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = configuration["Auth0:Audience"],
                    ValidIssuer = $"https://{configuration["Auth0:Domain"]}/",
                    RoleClaimType = $"{configuration["Auth0:Audience"]}/roles"
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(Policies.User, policy => policy.RequireRole(Policies.User))
                .AddPolicy(Policies.Admin, policy => policy.RequireRole(Policies.Admin));

            return services;
        }

        public IServiceCollection AddImmediate()
        {
            services.AddApiBehaviors();
            services.AddApiHandlers();

            return services;
        }

        public IServiceCollection AddInfrastructure(
            WebApplicationBuilder builder)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();

            services.AddDbContext<ApplicationDbContext>((
                sp,
                opt) =>
            {
                opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                if (builder.Environment.IsDevelopment())
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }

                opt.UseNpgsql(builder.Configuration.GetConnectionString(AppHostConstants.Database));
            });

            return services;
        }
    }
}