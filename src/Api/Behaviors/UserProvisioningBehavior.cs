using System.Security.Claims;
using Api.Features.Users.Model;

namespace Api.Behaviors;

public sealed class UserProvisioningBehavior<TRequest, TResponse>(
    ILogger<UserProvisioningBehavior<TRequest, TResponse>> logger,
    CurrentUserService currentUserService,
    ApplicationDbContext context,
    IHttpContextAccessor accessor
) : Behavior<TRequest, TResponse>
{
    public override async ValueTask<TResponse> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        var auth0Id = currentUserService.GetAuth0Id();

        var user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Auth0Id == auth0Id, cancellationToken);

        if (user is null)
        {
            user = User.Create(
                email: currentUserService.GetClaimValue<string>(ClaimTypes.Email),
                auth0Id: auth0Id
            );

            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);
        }

        accessor.HttpContext!.Items["UserId"] = user.Id;

        return await Next(request, cancellationToken);
    }
}
