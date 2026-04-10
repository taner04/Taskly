using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Taskly.Shared.Attributes;

namespace Taskly.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection RegisterAutoServices(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttributes(false)
                    .OfType<ServiceInjectionAttribute>()
                    .FirstOrDefault();

                if (attr is null)
                {
                    continue;
                }

                var lifetime = (ServiceLifetime)(int)attr.ServiceLifetime;
                var serviceType = attr.GetType() == typeof(ServiceInjectionAttribute)
                    ? type
                    : attr.GetType().GetGenericArguments()[1];

                services.Add(new ServiceDescriptor(serviceType, type, lifetime));
            }

            return services;
        }
    }
}