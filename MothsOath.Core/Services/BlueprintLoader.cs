using MothsOath.Core.Models.Blueprints.Common;
using System.Text.Json;

namespace MothsOath.Core.Services;

public class BlueprintLoader
{
    public Dictionary<string, T> LoadAllBlueprintsFromFiles<T>(string folderPath) where T : IBlueprint
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

    public Dictionary<string, JsonElement> LoadAllRawBlueprints(string folderPath)
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
}
