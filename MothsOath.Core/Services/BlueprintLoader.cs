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
        // Detecta se está rodando no WebAssembly verificando se HttpClient foi fornecido
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
            // No WebAssembly, não podemos usar GetAwaiter().GetResult() porque bloqueia
            // Este método não deve ser chamado no WebAssembly - use LoadAllBlueprintsFromFilesAsync
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

        var blueprintFiles = Directory.GetFiles(absoluteFolderPath, "*.json");
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

        // Lista conhecida de arquivos JSON - no WebAssembly não podemos listar diretórios
        // Vamos tentar carregar arquivos comuns ou usar uma lista pré-definida
        var knownFiles = GetKnownBlueprintFiles(folderPath);

        // Normaliza o nome da pasta para a URL (NPCs -> NPCs, mas aceita npcs também)
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
        // Normaliza o nome da pasta para corresponder ao case correto no sistema de arquivos
        var normalized = folderPath.ToLower().Trim();
        return normalized switch
        {
            "npcs" or "npc" => "NPCs",
            "races" => "Races",
            "archetypes" => "Archetypes",
            "cards" => "Cards",
            "diseases" => "Diseases",
            _ => folderPath // Mantém o original se não conhecido
        };
    }

    private List<string> GetKnownBlueprintFiles(string folderPath)
    {
        // Retorna lista de arquivos conhecidos baseado na pasta (case-insensitive)
        var normalizedPath = folderPath.ToLower().Trim();
        return normalizedPath switch
        {
            "races" => new List<string> { "ghoul_race.json", "human_race.json", "yulkin_race.json" },
            "archetypes" => new List<string> { "bellRinger_archetype.json", "doctor_archetype.json", "narrator_archetype.json" },
            "cards" => new List<string> { "echo_of_misfortune_card.json", "heal_card.json", "karma_calling_card.json", "sharp_cut_card.json", "strike_card.json", "toxic_jab_card.json" },
            "npcs" or "npc" => new List<string> { "narrator_extra.json", "neko.json", "skeleton.json" },
            "diseases" => new List<string> { "virus_disease.json" },
            _ => new List<string>()
        };
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
            // No WebAssembly, não podemos usar GetAwaiter().GetResult() porque bloqueia
            // Este método não deve ser chamado no WebAssembly - use LoadAllRawBlueprintsAsync
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

        var blueprintFiles = Directory.GetFiles(absoluteFolderPath, "*.json");
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

        var knownFiles = GetKnownBlueprintFiles(folderPath);
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
}
