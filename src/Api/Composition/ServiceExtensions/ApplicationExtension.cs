using Api.Features.Users;

namespace Api.Composition.ServiceExtensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CurrentUserService>();

        return services;
    }
}