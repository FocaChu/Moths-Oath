using System.Text.Json;

namespace MothsOath.Core.Services.Repositories;

/// <summary>
/// Loads blueprint data from the local file system.
/// Used for Desktop/Server environments.
/// </summary>
public class FileSystemDataSource : IBlueprintDataSource
{
    private readonly string _baseDirectory;
    
    public FileSystemDataSource(string baseDirectory)
    {
        _baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
    }
    
    public async Task<Dictionary<string, string>> LoadRawBlueprintsAsync(string folderPath)
    {
        var results = new Dictionary<string, string>();
        
        var absolutePath = Path.Combine(_baseDirectory, "Data", "Blueprints", folderPath);
        
        if (!Directory.Exists(absolutePath))
        {
            return results;
        }
        
        // Try to load from manifest first
        var manifestFiles = await LoadManifestFilesAsync(absolutePath);
        
        var files = manifestFiles.Any()
            ? manifestFiles.Select(f => Path.Combine(absolutePath, f)).Where(File.Exists)
            : Directory.GetFiles(absolutePath, "*.json").Where(f => !f.EndsWith("manifest.json"));
        
        foreach (var filePath in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var fileName = Path.GetFileName(filePath);
                results[fileName] = json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Failed to read file '{Path.GetFileName(filePath)}': {ex.Message}");
            }
        }
        
        return results;
    }
    
    public async Task<string?> LoadRawBlueprintAsync(string folderPath, string fileName)
    {
        var absolutePath = Path.Combine(_baseDirectory, "Data", "Blueprints", folderPath, fileName);
        
        if (!File.Exists(absolutePath))
        {
            return null;
        }
        
        try
        {
            return await File.ReadAllTextAsync(absolutePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] Failed to read file '{fileName}': {ex.Message}");
            return null;
        }
    }
    
    public Task<bool> IsAvailableAsync()
    {
        var dataPath = Path.Combine(_baseDirectory, "Data", "Blueprints");
        return Task.FromResult(Directory.Exists(dataPath));
    }
    
    private async Task<List<string>> LoadManifestFilesAsync(string folderPath)
    {
        var manifestPath = Path.Combine(folderPath, "manifest.json");
        
        if (!File.Exists(manifestPath))
        {
            return new List<string>();
        }
        
        try
        {
            var json = await File.ReadAllTextAsync(manifestPath);
            var manifest = JsonSerializer.Deserialize<BlueprintManifest>(
                json, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return manifest?.Files ?? new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] Failed to read manifest.json in '{folderPath}': {ex.Message}");
            return new List<string>();
        }
    }
}
