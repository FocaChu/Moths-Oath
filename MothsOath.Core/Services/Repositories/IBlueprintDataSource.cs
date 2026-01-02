namespace MothsOath.Core.Services.Repositories;

/// <summary>
/// Abstraction for loading raw blueprint data from different sources.
/// Implementations can load from FileSystem, HTTP, Database, etc.
/// </summary>
public interface IBlueprintDataSource
{
    /// <summary>
    /// Loads raw JSON content of all blueprints from a folder.
    /// </summary>
    /// <param name="folderPath">Relative folder path (e.g., "NPCs", "Archetypes")</param>
    /// <returns>Dictionary mapping filename to JSON content</returns>
    Task<Dictionary<string, string>> LoadRawBlueprintsAsync(string folderPath);
    
    /// <summary>
    /// Loads raw JSON content of a specific blueprint file.
    /// </summary>
    /// <param name="folderPath">Relative folder path</param>
    /// <param name="fileName">Blueprint file name</param>
    /// <returns>JSON content or null if not found</returns>
    Task<string?> LoadRawBlueprintAsync(string folderPath, string fileName);
    
    /// <summary>
    /// Checks if the data source is available and ready.
    /// </summary>
    Task<bool> IsAvailableAsync();
}
