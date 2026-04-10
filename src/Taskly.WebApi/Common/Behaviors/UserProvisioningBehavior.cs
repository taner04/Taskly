using System.Security.Claims;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Features.Users.Models;

namespace Taskly.WebApi.Common.Behaviors;

public sealed partial class UserProvisioningBehavior<TRequest, TResponse>(
    ILogger<UserProvisioningBehavior<TRequest, TResponse>> logger,
    CurrentUserService currentUserService,
    TasklyDbContext context,
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
            LogUserNotFound(auth0Id);

            user = User.Create(
                currentUserService.GetClaimValue<string>(ClaimTypes.Email),
                auth0Id
            );

            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            LogUserCreated(auth0Id);
        }
        else
        {
            LogUserFound(auth0Id);
        }

        accessor.HttpContext!.Items["UserId"] = user.Id;

        return await Next(request, cancellationToken);
    }

    [LoggerMessage(0, LogLevel.Warning, "User with Auth0Id '{Auth0Id}' not found.")]
    private partial void LogUserNotFound(string auth0Id);

    [LoggerMessage(1, LogLevel.Information, "User with Auth0Id '{Auth0Id}' created.")]
    private partial void LogUserCreated(string auth0Id);

    [LoggerMessage(2, LogLevel.Information, "User with Auth0Id '{Auth0Id}' was found.")]
    private partial void LogUserFound(string auth0Id);
}