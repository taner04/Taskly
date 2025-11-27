namespace Api.Composition.ServiceExtensions;

internal static class ImmediateExtension
{
    internal static IServiceCollection AddImmediate(this IServiceCollection services)
    {
        services.AddApiBehaviors();
        services.AddApiHandlers();

        return services;
    }
}