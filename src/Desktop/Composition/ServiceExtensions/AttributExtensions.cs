using Desktop.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
                t.IsClass &&
                !t.IsAbstract &&
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
