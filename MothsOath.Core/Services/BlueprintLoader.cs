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

    public async Task<Dictionary<string, T>> LoadAllBlueprintsFromFilesAsync<T>(string folderPath) where T : IBlueprint
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

    public Dictionary<string, T> LoadAllBlueprintsFromFiles<T>(string folderPath) where T : IBlueprint
    {
        if (_isWebAssembly && _httpClient != null)
        {
            throw new InvalidOperationException(
                "LoadAllBlueprintsFromFiles não pode ser usado no WebAssembly. Use LoadAllBlueprintsFromFilesAsync e aguarde a inicialização assíncrona.");
        }
        else
        {
            return LoadFromFileSystem<T>(folderPath);
        }
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
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var blueprint = System.Text.Json.JsonSerializer.Deserialize<T>(json, options);

                if (blueprint == null || string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    throw new InvalidDataException("O blueprint deserializado é nulo ou não possui um 'Id' válido.");
                }

                if (loadedBlueprints.ContainsKey(blueprint.Id))
                {
                    throw new InvalidDataException($"Um blueprint com o Id '{blueprint.Id}' já foi carregado. IDs devem ser únicos para o tipo '{typeof(T).Name}'.");
                }

                loadedBlueprints[blueprint.Id] = blueprint;
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
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var blueprint = System.Text.Json.JsonSerializer.Deserialize<T>(json, options);

                if (blueprint == null || string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    throw new InvalidDataException("O blueprint deserializado é nulo ou não possui um 'Id' válido.");
                }

                if (loadedBlueprints.ContainsKey(blueprint.Id))
                {
                    throw new InvalidDataException($"Um blueprint com o Id '{blueprint.Id}' já foi carregado. IDs devem ser únicos para o tipo '{typeof(T).Name}'.");
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

        try
        {
            if (File.Exists(manifestPath))
            {
                var json = File.ReadAllText(manifestPath);
                var manifest = JsonSerializer.Deserialize<BlueprintManifest>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return manifest?.Files ?? new List<string>();
            }
            else
            {
                Console.WriteLine($"[INFO] Manifest não encontrado em '{manifestPath}'. Usando descoberta automática de arquivos.");
                return new List<string>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AVISO] Falha ao ler manifest de '{manifestPath}'. Usando descoberta automática. Detalhes: {ex.Message}");
            return new List<string>();
        }
    }

    public async Task<Dictionary<string, JsonElement>> LoadAllRawBlueprintsAsync(string folderPath)
    {
        if (_isWebAssembly && _httpClient != null)
        {
            return await LoadRawFromHttpAsync(folderPath);
        }
        else
        {
            return LoadRawFromFileSystem(folderPath);
        }
    }

    public Dictionary<string, JsonElement> LoadAllRawBlueprints(string folderPath)
    {
        if (_isWebAssembly && _httpClient != null)
        {
            throw new InvalidOperationException(
                "LoadAllRawBlueprints não pode ser usado no WebAssembly. Use LoadAllRawBlueprintsAsync e aguarde a inicialização assíncrona.");
        }
        else
        {
            return LoadRawFromFileSystem(folderPath);
        }
    }

    private Dictionary<string, JsonElement> LoadRawFromFileSystem(string folderPath)
    {
        var loadedBlueprints = new Dictionary<string, JsonElement>();

        string assemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
        string absoluteFolderPath = Path.Combine(assemblyLocation, "Data", "Blueprints", folderPath);

        if (!Directory.Exists(absoluteFolderPath))
        {
            Console.WriteLine($"[AVISO] O diretório de blueprints não foi encontrado em: {absoluteFolderPath}. Nenhum blueprint será carregado.");
            return loadedBlueprints;
        }

        var manifestFiles = LoadManifestFromFileSystem(folderPath);
        var blueprintFiles = manifestFiles.Count > 0 
            ? manifestFiles.Select(f => Path.Combine(absoluteFolderPath, f)).Where(File.Exists).ToArray()
            : Directory.GetFiles(absoluteFolderPath, "*.json").Where(f => !f.EndsWith("manifest.json")).ToArray();

        Console.WriteLine($"--- Carregando Blueprints da Pasta: {folderPath} ---");
        Console.WriteLine($"Encontrados {blueprintFiles.Length} arquivos.");

        foreach (var file in blueprintFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string key = fileName;

            try
            {
                var json = File.ReadAllText(file);

                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    JsonElement root = document.RootElement.Clone();

                    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("Id", out JsonElement idElement))
                    {
                        if (idElement.ValueKind == JsonValueKind.String)
                        {
                            key = idElement.GetString() ?? fileName;
                        }
                    }

                    if (loadedBlueprints.ContainsKey(key))
                    {
                        Console.WriteLine($"[AVISO] ID/Nome '{key}' já carregado. Pulando o arquivo '{fileName}.json'. Verifique por IDs duplicados.");
                        continue;
                    }

                    loadedBlueprints[key] = root;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao processar o arquivo '{Path.GetFileName(file)}'. Detalhes: {ex.Message}");
            }
        }

        Console.WriteLine($"Carregamento genérico finalizado para a pasta '{folderPath}'. {loadedBlueprints.Count} de {blueprintFiles.Length} arquivos carregados com sucesso.");
        Console.WriteLine("--------------------------------------------------");
        return loadedBlueprints;
    }

    private async Task<Dictionary<string, JsonElement>> LoadRawFromHttpAsync(string folderPath)
    {
        var loadedBlueprints = new Dictionary<string, JsonElement>();

        if (_httpClient == null)
        {
            return loadedBlueprints;
        }

        var knownFiles = await LoadManifestFromHttpAsync(folderPath);
        var normalizedFolderPath = NormalizeFolderPath(folderPath);
        Console.WriteLine($"--- Carregando Blueprints da Pasta: {normalizedFolderPath} (WebAssembly) ---");
        Console.WriteLine($"Tentando carregar {knownFiles.Count} arquivos.");

        foreach (var fileName in knownFiles)
        {
            string key = Path.GetFileNameWithoutExtension(fileName);

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

                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    JsonElement root = document.RootElement.Clone();

                    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("Id", out JsonElement idElement))
                    {
                        if (idElement.ValueKind == JsonValueKind.String)
                        {
                            key = idElement.GetString() ?? key;
                        }
                    }

                    if (loadedBlueprints.ContainsKey(key))
                    {
                        Console.WriteLine($"[AVISO] ID/Nome '{key}' já carregado. Pulando o arquivo '{fileName}'. Verifique por IDs duplicados.");
                        continue;
                    }

                    loadedBlueprints[key] = root;
                    Console.WriteLine($"[OK] Carregado: {fileName} -> {key}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao processar o arquivo '{fileName}'. Detalhes: {ex.Message}");
            }
        }

        Console.WriteLine($"Carregamento genérico finalizado para a pasta '{folderPath}'. {loadedBlueprints.Count} de {knownFiles.Count} arquivos carregados com sucesso.");
        Console.WriteLine("--------------------------------------------------");
        return loadedBlueprints;
    }

    private class BlueprintManifest
    {
        public List<string> Files { get; set; } = new List<string>();
    }
}