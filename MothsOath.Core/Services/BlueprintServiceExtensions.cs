using Microsoft.Extensions.DependencyInjection;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services.Repositories;

namespace MothsOath.Core.Services;

/// <summary>
/// Extension methods for registering blueprint services in DI container.
/// </summary>
public static class BlueprintServiceExtensions
{
    /// <summary>
    /// Registers all blueprint-related services.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="isWebAssembly">True if running in WebAssembly, false for Desktop/Server</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddBlueprintSystem(
        this IServiceCollection services,
        bool isWebAssembly)
    {
        // Register appropriate data source based on environment
        if (isWebAssembly)
        {
            services.AddScoped<IBlueprintDataSource, HttpDataSource>();
        }
        else
        {
            services.AddSingleton<IBlueprintDataSource>(sp =>
                new FileSystemDataSource(AppDomain.CurrentDomain.BaseDirectory));
        }
        
        // Register blueprint repositories as singletons
        // They load once and cache for the lifetime of the application
        
        services.AddSingleton<IBlueprintRepository<NpcBlueprint>>(sp =>
            new BlueprintRepository<NpcBlueprint>(
                sp.GetRequiredService<IBlueprintDataSource>(),
                "NPCs"
            ));
        
        services.AddSingleton<IBlueprintRepository<ArchetypeBlueprint>>(sp =>
            new BlueprintRepository<ArchetypeBlueprint>(
                sp.GetRequiredService<IBlueprintDataSource>(),
                "Archetypes"
            ));
        
        services.AddSingleton<IBlueprintRepository<RaceBlueprint>>(sp =>
            new BlueprintRepository<RaceBlueprint>(
                sp.GetRequiredService<IBlueprintDataSource>(),
                "Races"
            ));
        
        services.AddSingleton<IBlueprintRepository<DiseaseBlueprint>>(sp =>
            new BlueprintRepository<DiseaseBlueprint>(
                sp.GetRequiredService<IBlueprintDataSource>(),
                "Diseases"
            ));
        
        // Register initialization service
        services.AddSingleton<GameInitializationService>();
        
        // Register adapter for backward compatibility with existing code
        services.AddSingleton<BlueprintCacheAdapter>();
        
        return services;
    }
}
