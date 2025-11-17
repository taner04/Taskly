using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Taskly.Api.Abstractions;

namespace Taskly.Api.Extensions;

public static class EndpointExtension
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapEndpoints()
        {
            var endpoints = app.ServiceProvider.GetService<IEnumerable<IEndpoint>>()?.ToList() ?? [];

            if (endpoints.Count == 0) throw new InvalidOperationException("No endpoints were found.");

            foreach (var endpoint in endpoints) endpoint.MapEndpoint(app);

            return app;
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddEndpoints()
        {
            var endpointTypes = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t is { IsAbstract: false, IsInterface: false }
                            && typeof(IEndpoint).IsAssignableFrom(t));

            foreach (var type in endpointTypes)
                services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IEndpoint), type));

            return services;
        }
    }
}