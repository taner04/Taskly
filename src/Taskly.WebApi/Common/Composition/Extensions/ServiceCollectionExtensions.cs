using System.Threading.RateLimiting;
using Azure.Storage.Blobs;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Taskly.ServiceDefaults;
using Taskly.Shared.Extensions;
using Taskly.WebApi.Common.Composition.Options;
using Taskly.WebApi.Common.Composition.Serialization;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Common.Infrastructure.Persistence.Interceptors;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Features.Todos.Models;

namespace Taskly.WebApi.Common.Composition.Extensions;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddApplication(
            IConfiguration configuration)
        {
            services.AddSingleton(_ =>
            {
                var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2024_11_04);

                return new BlobServiceClient(
                    configuration.GetConnectionString(AppHostConstants.AzureBlobStorage),
                    options);
            });

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
                options.MapInboundClaims =
                    false; // Prevents mapping of standard JWT claims to Microsoft-specific claim types

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = auth0Config.Audience,
                    ValidIssuer = $"https://{auth0Config.Domain}/",
                    RoleClaimType = CurrentUserService.RoleClaim,
                    NameClaimType = CurrentUserService.SubClaim
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(Policies.Roles.Admin,
                    policy => policy.RequireClaim("permissions", "admin:create", "admin:read"))
                .AddPolicy(Policies.Roles.User,
                    policy => policy.RequireClaim("permissions", "user:create", "user:read"));

            return services;
        }

        internal IServiceCollection AddRateLimiting()
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(Policies.RateLimiting.Global, context =>
                {
                    var userSub = context.User.FindFirst(CurrentUserService.SubClaim)?.Value;

                    return RateLimitPartition.GetTokenBucketLimiter(userSub, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100,
                        TokensPerPeriod = 100,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1)
                    });
                });

                //TODO: Ratelimit for webhook
            });

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

            var connectionString = builder.Configuration.GetConnectionString(AppHostConstants.Database);
            ArgumentNullException.ThrowIfNull(connectionString);

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

                opt.UseNpgsql(connectionString);
            });

            services.RegisterAutoServices(typeof(Program).Assembly);

            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(c =>
                    c.UseNpgsqlConnection(builder.Configuration.GetConnectionString(AppHostConstants.Database))));

            services.AddHangfireServer(options => { options.SchedulePollingInterval = TimeSpan.FromSeconds(1); });

            return services;
        }

        internal IServiceCollection AddCustomJsonConverter()
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new FlexibleEnumConverter<TodoPriority>());
                options.SerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
            });

            return services;
        }
    }
}