using System.Text.Json;

namespace MothsOath.Core.Services.Repositories;

/// <summary>
/// Loads blueprint data via HTTP requests.
/// Used for Blazor WebAssembly environments.
/// </summary>
public class HttpDataSource : IBlueprintDataSource
{
    private readonly HttpClient _httpClient;
    
    public HttpDataSource(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public async Task<Dictionary<string, string>> LoadRawBlueprintsAsync(string folderPath)
    {
        var results = new Dictionary<string, string>();
        
        var normalizedPath = NormalizeFolderPath(folderPath);
        
        // Load manifest to know which files to request
        var manifestFiles = await LoadManifestFilesAsync(normalizedPath);
        
        if (!manifestFiles.Any())
        {
            Console.WriteLine($"[WARN] No manifest found for '{folderPath}'. Cannot load blueprints via HTTP without manifest.");
            return results;
        }
        
        foreach (var fileName in manifestFiles)
        {
            try
            {
                var url = $"Data/Blueprints/{normalizedPath}/{fileName}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    results[fileName] = json;
                }
                else
                {
                    Console.WriteLine($"[WARN] File '{fileName}' not found at '{url}' (Status: {response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Failed to load file '{fileName}': {ex.Message}");
            }
        }
        
        return results;
    }
    
    public async Task<string?> LoadRawBlueprintAsync(string folderPath, string fileName)
    {
        var normalizedPath = NormalizeFolderPath(folderPath);
        var url = $"Data/Blueprints/{normalizedPath}/{fileName}";
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] Failed to load file '{fileName}': {ex.Message}");
            return null;
        }
    }
    
    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            // Test connectivity by trying to load a known path
            var response = await _httpClient.GetAsync("Data/Blueprints/");
            return true; // If we can make requests, source is available
        }
        catch
        {
            return false;
        }
    }
    
    private async Task<List<string>> LoadManifestFilesAsync(string folderPath)
    {
        try
        {
            var manifestUrl = $"Data/Blueprints/{folderPath}/manifest.json";
            var response = await _httpClient.GetAsync(manifestUrl);
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<string>();
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var manifest = JsonSerializer.Deserialize<BlueprintManifest>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return manifest?.Files ?? new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] Failed to load manifest for '{folderPath}': {ex.Message}");
            return new List<string>();
        }
    }
    
    private string NormalizeFolderPath(string folderPath)
    {
        var normalized = folderPath.ToLower().Trim();
        return normalized switch
        {
            "npcs" or "npc" => "NPCs",
            "races" => "Races",
            "archetypes" => "Archetypes",
            "cards" => "Cards",
            "diseases" => "Diseases",
            "tags" => "Tags",
            _ => folderPath
        };
    }
}
