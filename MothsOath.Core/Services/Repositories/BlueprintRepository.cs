using MothsOath.Core.Common.Exceptions;
using MothsOath.Core.Models.Blueprints.Common;
using System.Diagnostics;
using System.Text.Json;

namespace MothsOath.Core.Services.Repositories;

/// <summary>
/// Concrete implementation of blueprint repository.
/// Handles preloading, caching, deserialization, and validation.
/// </summary>
/// <typeparam name="T">Type of blueprint</typeparam>
public class BlueprintRepository<T> : IBlueprintRepository<T> where T : IBlueprint
{
    private readonly IBlueprintDataSource _dataSource;
    private readonly string _folderPath;
    
    // Cache - populated once during PreloadAsync
    private Dictionary<string, T>? _cache;
    private bool _isLoaded = false;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    
    public BlueprintRepository(IBlueprintDataSource dataSource, string folderPath)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _folderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));
    }
    
    public bool IsLoaded => _isLoaded;
    
    public IReadOnlyDictionary<string, T> All
    {
        get
        {
            ThrowIfNotLoaded();
            return _cache!;
        }
    }
    
    public async Task PreloadAsync()
    {
        await _loadLock.WaitAsync();
        try
        {
            if (_isLoaded)
            {
                Console.WriteLine($"[INFO] {typeof(T).Name} repository already loaded");
                return;
            }
            
            Console.WriteLine($"[INFO] Preloading {typeof(T).Name} blueprints from {_folderPath}");
            
            var startTime = Stopwatch.GetTimestamp();
            
            _cache = await LoadAndDeserializeAsync();
            _isLoaded = true;
            
            var elapsed = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"[INFO] Loaded {_cache.Count} {typeof(T).Name} blueprints in {elapsed.TotalMilliseconds:F2}ms");
        }
        finally
        {
            _loadLock.Release();
        }
    }
    
    public T Get(string id)
    {
        ThrowIfNotLoaded();
        
        if (_cache!.TryGetValue(id, out var blueprint))
        {
            return blueprint;
        }
        
        throw new BlueprintNotFoundException(id, typeof(T).Name);
    }
    
    public bool TryGet(string id, out T? blueprint)
    {
        blueprint = default;
        
        if (!_isLoaded)
        {
            Console.WriteLine($"[WARN] Attempted to access {typeof(T).Name} repository before preload");
            return false;
        }
        
        return _cache!.TryGetValue(id, out blueprint);
    }
    
    public bool Exists(string id)
    {
        ThrowIfNotLoaded();
        return _cache!.ContainsKey(id);
    }
    
    public async Task ReloadAsync()
    {
        await _loadLock.WaitAsync();
        try
        {
            Console.WriteLine($"[INFO] Reloading {typeof(T).Name} blueprints");
            _cache = await LoadAndDeserializeAsync();
            _isLoaded = true;
        }
        finally
        {
            _loadLock.Release();
        }
    }
    
    private void ThrowIfNotLoaded()
    {
        if (!_isLoaded)
        {
            throw new InvalidOperationException(
                $"Repository for {typeof(T).Name} was not preloaded. " +
                "Call PreloadAsync() during game initialization.");
        }
    }
    
    private async Task<Dictionary<string, T>> LoadAndDeserializeAsync()
    {
        var results = new Dictionary<string, T>();
        
        try
        {
            // Step 1: Load raw JSON data
            var rawData = await _dataSource.LoadRawBlueprintsAsync(_folderPath);
            
            Console.WriteLine($"[DEBUG] Found {rawData.Count} blueprint files of type {typeof(T).Name}");
            
            // Step 2: Deserialize each file
            foreach (var (fileName, json) in rawData)
            {
                try
                {
                    var blueprint = DeserializeBlueprint(json, fileName);
                    
                    if (blueprint == null)
                    {
                        Console.WriteLine($"[WARN] Blueprint {fileName} deserialized to null");
                        continue;
                    }
                    
                    // Step 3: Validate
                    ValidateBlueprint(blueprint, fileName);
                    
                    // Step 4: Check for duplicates
                    if (results.ContainsKey(blueprint.Id))
                    {
                        throw new InvalidBlueprintException(
                            blueprint.Id,
                            $"Duplicate ID found in file {fileName}");
                    }
                    
                    // Step 5: Add to cache
                    results[blueprint.Id] = blueprint;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"[ERROR] Failed to deserialize {fileName}: {ex.Message}");
                }
                catch (InvalidBlueprintException ex)
                {
                    Console.WriteLine($"[ERROR] Invalid blueprint in {fileName}: {ex.Message}");
                }
            }
            
            Console.WriteLine($"[INFO] Successfully loaded {results.Count}/{rawData.Count} blueprints of type {typeof(T).Name}");
            
            return results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to load blueprints from {_folderPath}: {ex.Message}");
            throw new BlueprintLoadException(_folderPath, "Unexpected error during loading", ex);
        }
    }
    
    private T? DeserializeBlueprint(string json, string fileName)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
        
        return JsonSerializer.Deserialize<T>(json, options);
    }
    
    private void ValidateBlueprint(T blueprint, string fileName)
    {
        if (string.IsNullOrWhiteSpace(blueprint.Id))
        {
            throw new InvalidBlueprintException(
                fileName,
                "Blueprint must have a valid Id");
        }
    }
}
