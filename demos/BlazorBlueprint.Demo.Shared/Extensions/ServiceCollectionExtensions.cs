using Microsoft.Extensions.DependencyInjection;
using BlazorBlueprint.Demo.Services;
using BlazorBlueprint.Primitives.Extensions;
using BlazorBlueprint.Components.Toast;

namespace BlazorBlueprint.Demo.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorBlueprintDemo(this IServiceCollection services)
    {
        // Add BlazorBlueprint.Primitives services
        services.AddBlazorBlueprintPrimitives();

        // Add theme service for dark mode management
        services.AddScoped<ThemeService>();

        // Add collapsible state service for menu state persistence
        services.AddScoped<CollapsibleStateService>();

        // Add mock data service for generating demo data
        services.AddSingleton<MockDataService>();

        // Add toast notification service
        services.AddScoped<ToastService>();

        return services;
    }
}
