namespace Api.Extensions;

public static class ImmediateExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddImmediate()
        {
            services.AddApiBehaviors();
            services.AddApiHandlers();

            return services;
        }
    }
}