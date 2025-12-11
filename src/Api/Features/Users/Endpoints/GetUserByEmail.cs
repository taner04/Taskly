using System.ComponentModel.DataAnnotations;
using Api.Features.Users.Exceptions;
using Api.Features.Users.Model;
using ValidationResult = Immediate.Validations.Shared.ValidationResult;

namespace Api.Features.Users.Endpoints;

[Handler]
[MapGet(Routes.Users.GetByEmail)]
[Authorize(Policy = Policies.Admin)]
public static partial class GetUserByEmail
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(User));
    }

    private static async ValueTask<User> HandleAsync(
        [AsParameters] Query query,
        ApplicationDbContext context,
        CancellationToken ct)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.Email == query.Email, ct);

        return user ?? throw new UserEmailNotFoundException(query.Email);
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
            if(!new EmailAddressAttribute().IsValid(target.Email))
            {
                errors.Add(nameof(target.Email), $"The email '{target.Email}' is not a valid email address.");
            }
        }
    }
}