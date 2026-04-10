using System.Reflection;

namespace Taskly.Desktop.Common.Composition.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPages(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var pages = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false }
                            && t.GetInterfaces().Any(i =>
                                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INavigableView<>))
                            && t.GetCustomAttribute<PageInjectionAttribute>(false) is not null)
                .ToList();

            foreach (var page in pages)
            {
                services.AddSingleton(page);

                var attribute = page.GetCustomAttribute<PageInjectionAttribute>(false);
                var viewModelType = attribute?.ViewModel ??
                                    throw new InvalidOperationException(
                                        $"PageInjectionAttribute not found on {page.FullName}.");
                services.AddSingleton(viewModelType);
            }

            return services;
        }
    }
}