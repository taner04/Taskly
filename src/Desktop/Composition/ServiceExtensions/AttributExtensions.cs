using System.Reflection;
using Desktop.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Desktop.Composition.ServiceExtensions;

public static class AttributExtensions
{
    public static IServiceCollection AddPagesFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        var pageTypes = assembly
            .GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.GetCustomAttribute<PageRegistrationAttribute>() is not null)
            .ToList();


        pageTypes.ForEach(pageType =>
        {
            var attribute = pageType.GetCustomAttribute<PageRegistrationAttribute>();
            ArgumentNullException.ThrowIfNull(attribute);

            // Register ViewModel
            services.AddSingleton(attribute.ViewModelType);

            // Register Page
            services.AddSingleton(pageType);
        });

        return services;
    }
}