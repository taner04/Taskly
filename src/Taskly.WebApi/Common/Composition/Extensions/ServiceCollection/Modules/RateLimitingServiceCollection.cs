using System.Threading.RateLimiting;

namespace Taskly.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class RateLimitingServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddTasklyRateLimiting()
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(Security.RateLimiting.Global, context =>
                {
                    var userSub = context.User.FindFirst(CurrentUserService.SubClaim)?.Value;

                    return RateLimitPartition.GetTokenBucketLimiter(userSub, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100,
                        TokensPerPeriod = 100,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1)
                    });
                });
            });

            return services;
        }
    }
}