using MothsOath.Core.Models.Blueprints.Common;

namespace MothsOath.Core.Services.Repositories;

/// <summary>
/// Repository for managing blueprints with synchronous runtime access.
/// MUST call PreloadAsync() during game initialization before using Get() methods.
/// </summary>
/// <typeparam name="T">Type of blueprint</typeparam>
public interface IBlueprintRepository<T> where T : IBlueprint
{
    /// <summary>
    /// Preloads ALL blueprints asynchronously.
    /// MUST be called during game initialization (loading screen).
    /// </summary>
    Task PreloadAsync();
    
    /// <summary>
    /// Gets a blueprint by ID (synchronous).
    /// Throws exception if not found or if PreloadAsync was not called.
    /// Use this in gameplay code.
    /// </summary>
    /// <param name="id">Blueprint ID</param>
    /// <returns>The blueprint</returns>
    /// <exception cref="InvalidOperationException">If repository was not preloaded</exception>
    /// <exception cref="Common.Exceptions.BlueprintNotFoundException">If blueprint with ID not found</exception>
    T Get(string id);
    
    /// <summary>
    /// Tries to get a blueprint by ID without throwing exceptions.
    /// Returns false if not found or not preloaded.
    /// </summary>
    /// <param name="id">Blueprint ID</param>
    /// <param name="blueprint">Output blueprint</param>
    /// <returns>True if found, false otherwise</returns>
    bool TryGet(string id, out T? blueprint);
    
    /// <summary>
    /// Checks if a blueprint with the given ID exists.
    /// </summary>
    /// <param name="id">Blueprint ID</param>
    /// <returns>True if exists</returns>
    /// <exception cref="InvalidOperationException">If repository was not preloaded</exception>
    bool Exists(string id);
    
    /// <summary>
    /// Gets read-only access to ALL loaded blueprints.
    /// </summary>
    /// <exception cref="InvalidOperationException">If repository was not preloaded</exception>
    IReadOnlyDictionary<string, T> All { get; }
    
    /// <summary>
    /// Indicates whether the repository has been preloaded.
    /// </summary>
    bool IsLoaded { get; }
    
    /// <summary>
    /// Forces a reload of all blueprints (useful for hot-reload in development).
    /// </summary>
    Task ReloadAsync();
}
