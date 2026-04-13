using System.ComponentModel.DataAnnotations;
using Taskly.WebApi.Features.Users.Exceptions;
using ValidationResult = Immediate.Validations.Shared.ValidationResult;

namespace Taskly.WebApi.Features.Users.Endpoints;

[Handler]
[MapGet(ApiRoutes.Users.GetByEmail)]
[Authorize(Policy = Policies.Roles.Admin)]
public static partial class GetUserByEmail
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(User));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask<User> HandleAsync(
        [AsParameters] Query query,
        TasklyDbContext context,
        CancellationToken ct)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.Email == query.Email, ct) ?? throw new UserNotFoundException(query.Email);

        return user;
    }

    [Validate]
    public sealed partial record Query : IValidationTarget<Query>
    {
        public required string Email { get; set; }

        private static void AdditionalValidations(
            ValidationResult errors,
            Query target
        )
        {
            if (!new EmailAddressAttribute().IsValid(target.Email))
            {
                errors.Add(nameof(target.Email), $"The email '{target.Email}' is not a valid email address.");
            }
        }
    }
}