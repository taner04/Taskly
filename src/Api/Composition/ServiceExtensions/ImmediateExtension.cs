namespace Api.Composition.ServiceExtensions;

internal static class ImmediateExtension
{
    extension(
        IServiceCollection services)
    {
        internal IServiceCollection AddImmediate()
        {
            services.AddApiBehaviors();
            services.AddApiHandlers();

            return services;
        }
    }
}