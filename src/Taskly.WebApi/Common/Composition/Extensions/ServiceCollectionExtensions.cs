using Taskly.WebApi.Common.Composition.Options;
using Taskly.WebApi.Common.Composition.Serialization;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Common.Infrastructure.Persistence.Interceptors;
using Taskly.WebApi.Features.Attachments.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Taskly.ServiceDefaults;
using Taskly.Shared.Extensions;

namespace Taskly.WebApi.Common.Composition.Extensions;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddApplication(
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

        internal IServiceCollection AddAuthenticationAndAuthorization(
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

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = auth0Config.Audience,
                    ValidIssuer = $"https://{auth0Config.Domain}/"
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(Policies.Admin, policy => policy.RequireClaim("permissions", "admin:create", "admin:read"))
                .AddPolicy(Policies.User, policy => policy.RequireClaim("permissions", "user:create", "user:read"));

            return services;
        }

        internal IServiceCollection AddImmediate()
        {
            services.AddTasklyWebApiBehaviors();
            services.AddTasklyWebApiHandlers();

            return services;
        }

        internal IServiceCollection AddInfrastructure(
            WebApplicationBuilder builder)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();

            services.AddDbContext<TasklyDbContext>((
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

        internal IServiceCollection AddCustomJsonConverter()
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new FlexibleEnumConverter<TodoPriority>());
            });

            return services;
        }
    }
}
