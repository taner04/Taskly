namespace Api.Composition.ServiceExtensions;

public static class ImmediateExtension
{
    public static IServiceCollection AddImmediate(this IServiceCollection services)
    {
        services.AddApiBehaviors();
        services.AddApiHandlers();

        return services;
    }
}