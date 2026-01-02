using MothsOath.Core.Common.Exceptions;
using MothsOath.Core.Models.Blueprints.Common;
using System.Text.Json;

namespace MothsOath.Core.Services;

public class BlueprintLoader
{
    private readonly HttpClient? _httpClient;
    private readonly bool _isWebAssembly;

    public BlueprintLoader(HttpClient? httpClient = null)
    {
        _httpClient = httpClient;
        _isWebAssembly = httpClient != null;
    }

    public async Task<Dictionary<string, JsonElement>> LoadAllBlueprintsAsJsonAsync(string folderPath)
    {
        try
        {
            if (_isWebAssembly && _httpClient != null)
            {
                return await LoadAsJsonFromHttpAsync(folderPath);
            }
            else
            {
                return LoadAsJsonFromFileSystem(folderPath);
            }
        }
        catch (Exception ex)
        {
            throw new BlueprintLoadException(folderPath, "Unexpected error loading blueprints as JSON", ex);
        }
    }

    public async Task<Dictionary<string, T>> LoadAllBlueprintsFromFilesAsync<T>(string folderPath) where T : IBlueprint
    {
        try
        {
            if (_isWebAssembly && _httpClient != null)
            {
                return await LoadFromHttpAsync<T>(folderPath);
            }
            else
            {
                return LoadFromFileSystem<T>(folderPath);
            }
        }
        catch (Exception ex) when (ex is not BlueprintLoadException)
        {
            throw new BlueprintLoadException(folderPath, "Unexpected error loading blueprints", ex);
        }
    }

    public Dictionary<string, T> LoadAllBlueprintsFromFiles<T>(string folderPath) where T : IBlueprint
    {
        if (_isWebAssembly && _httpClient != null)
        {
            throw new InvalidOperationException(
                "LoadAllBlueprintsFromFiles não pode ser usado no WebAssembly. Use LoadAllBlueprintsFromFilesAsync e aguarde a inicialização assíncrona.");
        }

        try
        {
            return LoadFromFileSystem<T>(folderPath);
        }
        catch (Exception ex) when (ex is not BlueprintLoadException)
        {
            throw new BlueprintLoadException(folderPath, "Unexpected error loading blueprints", ex);
        }
    }

    private Dictionary<string, JsonElement> LoadAsJsonFromFileSystem(string folderPath)
    {
        var loadedBlueprints = new Dictionary<string, JsonElement>();

        string assemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
        string absoluteFolderPath = Path.Combine(assemblyLocation, "Data", "Blueprints", folderPath);

        if (!Directory.Exists(absoluteFolderPath))
        {
            Console.WriteLine($"[AVISO] O diretório de blueprints não foi encontrado em: {absoluteFolderPath}. Nenhum blueprint será carregado de '{folderPath}'.");
            return loadedBlueprints;
        }

        var manifestFiles = LoadManifestFromFileSystem(folderPath);
        var blueprintFiles = manifestFiles.Count > 0
            ? manifestFiles.Select(f => Path.Combine(absoluteFolderPath, f)).Where(File.Exists).ToArray()
            : Directory.GetFiles(absoluteFolderPath, "*.json").Where(f => !f.EndsWith("manifest.json")).ToArray();

        Console.WriteLine($"--- Carregando Blueprints JSON de '{folderPath}' ---");
        Console.WriteLine($"Encontrados {blueprintFiles.Length} arquivos.");

        foreach (var file in blueprintFiles)
        {
            try
            {
                var json = File.ReadAllText(file);
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("Id", out var idElement))
                {
                    Console.WriteLine($"[ERRO] Arquivo '{Path.GetFileName(file)}' não possui propriedade 'Id'.");
                    continue;
                }

                string? id = idElement.GetString();
                if (string.IsNullOrWhiteSpace(id))
                {
                    Console.WriteLine($"[ERRO] Arquivo '{Path.GetFileName(file)}' possui 'Id' vazio ou nulo.");
                    continue;
                }

                if (loadedBlueprints.ContainsKey(id))
                {
                    Console.WriteLine($"[ERRO] Blueprint com Id '{id}' já foi carregado. IDs devem ser únicos.");
                    continue;
                }

                loadedBlueprints[id] = root.Clone();
                Console.WriteLine($"[OK] Carregado: {Path.GetFileName(file)} -> {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao processar '{Path.GetFileName(file)}': {ex.Message}");
            }
        }

        Console.WriteLine($"Carregamento finalizado. {loadedBlueprints.Count} de {blueprintFiles.Length} blueprints carregados.");
        Console.WriteLine("--------------------------------------------------");
        return loadedBlueprints;
    }

    private async Task<Dictionary<string, JsonElement>> LoadAsJsonFromHttpAsync(string folderPath)
    {
        var loadedBlueprints = new Dictionary<string, JsonElement>();

        if (_httpClient == null)
        {
            return loadedBlueprints;
        }

        var knownFiles = await LoadManifestFromHttpAsync(folderPath);
        var normalizedFolderPath = NormalizeFolderPath(folderPath);

        Console.WriteLine($"--- Carregando Blueprints JSON de '{normalizedFolderPath}' (WebAssembly) ---");
        Console.WriteLine($"Tentando carregar {knownFiles.Count} arquivos da pasta '{normalizedFolderPath}'.");

        foreach (var fileName in knownFiles)
        {
            try
            {
                var url = $"Data/Blueprints/{normalizedFolderPath}/{fileName}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[AVISO] Arquivo '{fileName}' não encontrado em '{url}'.");
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("Id", out var idElement))
                {
                    Console.WriteLine($"[ERRO] Arquivo '{fileName}' não possui propriedade 'Id'.");
                    continue;
                }

                string? id = idElement.GetString();
                if (string.IsNullOrWhiteSpace(id))
                {
                    Console.WriteLine($"[ERRO] Arquivo '{fileName}' possui 'Id' vazio ou nulo.");
                    continue;
                }

                if (loadedBlueprints.ContainsKey(id))
                {
                    Console.WriteLine($"[ERRO] Blueprint com Id '{id}' já foi carregado. IDs devem ser únicos.");
                    continue;
                }

                loadedBlueprints[id] = root.Clone();
                Console.WriteLine($"[OK] Carregado: {fileName} -> {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao processar '{fileName}': {ex.Message}");
            }
        }

        Console.WriteLine($"Carregamento finalizado. {loadedBlueprints.Count} de {knownFiles.Count} blueprints carregados com sucesso.");
        Console.WriteLine("--------------------------------------------------");
        return loadedBlueprints;
    }

    private Dictionary<string, T> LoadFromFileSystem<T>(string folderPath) where T : IBlueprint
    {
        var loadedBlueprints = new Dictionary<string, T>();

        string assemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
        string absoluteFolderPath = Path.Combine(assemblyLocation, "Data", "Blueprints", folderPath);

        if (!Directory.Exists(absoluteFolderPath))
        {
            Console.WriteLine($"[AVISO] O diretório de blueprints não foi encontrado em: {absoluteFolderPath}. Nenhum blueprint do tipo '{typeof(T).Name}' será carregado.");
            return loadedBlueprints;
        }

        var manifestFiles = LoadManifestFromFileSystem(folderPath);
        var blueprintFiles = manifestFiles.Count > 0 
            ? manifestFiles.Select(f => Path.Combine(absoluteFolderPath, f)).Where(File.Exists).ToArray()
            : Directory.GetFiles(absoluteFolderPath, "*.json").Where(f => !f.EndsWith("manifest.json")).ToArray();

        Console.WriteLine($"--- Carregando Blueprints do Tipo: {typeof(T).Name} ---");
        Console.WriteLine($"Encontrados {blueprintFiles.Length} arquivos em '{folderPath}'.");

        foreach (var file in blueprintFiles)
        {
            try
            {
                var json = File.ReadAllText(file);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var blueprint = JsonSerializer.Deserialize<T>(json, options);

                if (blueprint == null || string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    throw new InvalidBlueprintException(
                        Path.GetFileName(file), 
                        "O blueprint deserializado é nulo ou não possui um 'Id' válido.");
                }

                if (loadedBlueprints.ContainsKey(blueprint.Id))
                {
                    throw new InvalidBlueprintException(
                        blueprint.Id, 
                        $"Um blueprint com o Id '{blueprint.Id}' já foi carregado. IDs devem ser únicos para o tipo '{typeof(T).Name}'.");
                }

                loadedBlueprints[blueprint.Id] = blueprint;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[ERRO] Falha ao deserializar o arquivo '{Path.GetFileName(file)}'. JSON inválido: {ex.Message}");
            }
            catch (InvalidBlueprintException ex)
            {
                Console.WriteLine($"[ERRO] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao processar o arquivo '{Path.GetFileName(file)}'. Detalhes: {ex.Message}");
            }
        }

        Console.WriteLine($"Carregamento finalizado para '{typeof(T).Name}'. {loadedBlueprints.Count} de {blueprintFiles.Length} blueprints carregados com sucesso.");
        Console.WriteLine("--------------------------------------------------");
        return loadedBlueprints;
    }

    private async Task<Dictionary<string, T>> LoadFromHttpAsync<T>(string folderPath) where T : IBlueprint
    {
        var loadedBlueprints = new Dictionary<string, T>();

        if (_httpClient == null)
        {
            return loadedBlueprints;
        }

        var knownFiles = await LoadManifestFromHttpAsync(folderPath);
        var normalizedFolderPath = NormalizeFolderPath(folderPath);

        Console.WriteLine($"--- Carregando Blueprints do Tipo: {typeof(T).Name} (WebAssembly) ---");
        Console.WriteLine($"Tentando carregar {knownFiles.Count} arquivos da pasta '{normalizedFolderPath}'.");

        foreach (var fileName in knownFiles)
        {
            try
            {
                var url = $"Data/Blueprints/{normalizedFolderPath}/{fileName}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[AVISO] Arquivo '{fileName}' não encontrado em '{url}'.");
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var blueprint = JsonSerializer.Deserialize<T>(json, options);

                if (blueprint == null || string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    throw new InvalidBlueprintException(fileName, "O blueprint deserializado é nulo ou não possui um 'Id' válido.");
                }

                if (loadedBlueprints.ContainsKey(blueprint.Id))
                {
                    throw new InvalidBlueprintException(blueprint.Id, $"Um blueprint com o Id '{blueprint.Id}' já foi carregado. IDs devem ser únicos para o tipo '{typeof(T).Name}'.");
                }

                loadedBlueprints[blueprint.Id] = blueprint;
                Console.WriteLine($"[OK] Carregado: {fileName} -> {blueprint.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao processar o arquivo '{fileName}'. Detalhes: {ex.Message}");
            }
        }

        Console.WriteLine($"Carregamento finalizado para '{typeof(T).Name}'. {loadedBlueprints.Count} de {knownFiles.Count} blueprints carregados com sucesso.");
        Console.WriteLine("--------------------------------------------------");
        return loadedBlueprints;
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

    private async Task<List<string>> LoadManifestFromHttpAsync(string folderPath)
    {
        if (_httpClient == null)
        {
            return new List<string>();
        }

        var normalizedFolderPath = NormalizeFolderPath(folderPath);

        try
        {
            var manifestUrl = $"Data/Blueprints/{normalizedFolderPath}/manifest.json";
            var response = await _httpClient.GetAsync(manifestUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[AVISO] Manifest não encontrado em '{manifestUrl}'. Retornando lista vazia.");
                return new List<string>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var manifest = JsonSerializer.Deserialize<BlueprintManifest>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return manifest?.Files ?? new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha ao carregar manifest de '{folderPath}'. Detalhes: {ex.Message}");
            return new List<string>();
        }
    }

    private List<string> LoadManifestFromFileSystem(string folderPath)
    {
        string assemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
        string absoluteFolderPath = Path.Combine(assemblyLocation, "Data", "Blueprints", folderPath);
        string manifestPath = Path.Combine(absoluteFolderPath, "manifest.json");

        if (!File.Exists(manifestPath))
        {
            return new List<string>();
        }

        try
        {
            var json = File.ReadAllText(manifestPath);
            var manifest = JsonSerializer.Deserialize<BlueprintManifest>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return manifest?.Files ?? new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AVISO] Falha ao ler manifest.json em '{manifestPath}'. Detalhes: {ex.Message}");
            return new List<string>();
        }
    }
}

public class BlueprintManifest
{
    public List<string> Files { get; set; } = new List<string>();
}
