using Api.Common.Composition.Options;
using Api.Common.Composition.Serialization;
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
            var auth0Config = configuration.GetSection(nameof(Auth0Config)).Get<Auth0Config>();
            ArgumentNullException.ThrowIfNull(auth0Config);

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

        public IServiceCollection AddCustomJsonConverter()
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new FlexibleEnumConverter<TodoPriority>());
            });

            return services;
        }
    }
}