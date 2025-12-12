using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Abstractions.Controls;
using Desktop.Attributes;

namespace Desktop.Extensions;

public static class RegisterPagesExtension
{
    public static IServiceCollection AddPages(this IServiceCollection services, Assembly assembly)
    {
        if (assembly is null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        var pages = assembly.GetTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                        && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INavigableView<>))
                        && t.GetCustomAttribute<PageInjectionAttribute>(inherit: false) is not null)
            .ToList();
        
        foreach (var page in pages)
        {
            services.AddSingleton(page);

            var attribute = page.GetCustomAttribute<PageInjectionAttribute>(inherit: false);
            var viewModelType = attribute?.ViewModel ?? throw new InvalidOperationException($"PageInjectionAttribute not found on {page.FullName}.");
            services.AddSingleton(viewModelType);
        }

        return services;
    }
}
