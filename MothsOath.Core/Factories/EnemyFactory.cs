using MothsOath.Core.Entities;
using MothsOath.Core.Models.Blueprints;

namespace MothsOath.Core.Factories;

public class EnemyFactory
{
    private readonly Dictionary<string, EnemyBlueprint> _enemyBlueprints;
    private readonly AbilityFactory _abilityFactory; 

    public EnemyFactory(AbilityFactory abilityFactory)
    {
        _abilityFactory = abilityFactory;
        _enemyBlueprints = LoadAllBlueprintsFromFiles();
    }

    public Enemy CreateEnemy(string blueprintId)
    {
        if (!_enemyBlueprints.TryGetValue(blueprintId, out var blueprint))
        {
            throw new Exception($"Blueprint de inimigo '{blueprintId}' não encontrado!");
        }

        var enemy = new Enemy
        {
            Name = blueprint.Name,
            MaxHP = blueprint.MaxHP,
            CurrentHP = blueprint.MaxHP,
            BaseStrength = blueprint.BaseStrength,
            BasicAttack = _abilityFactory.GetAbility(blueprint.BasicAttackAbilityId),
            SpecialAbility = _abilityFactory.GetAbility(blueprint.SpecialAbilityId),
            SpecialAbilityCooldown = blueprint.SpecialAbilityCooldown
        };

        return enemy;
    }

    private Dictionary<string, EnemyBlueprint> LoadAllBlueprintsFromFiles()
    {
        var loadedBlueprints = new Dictionary<string, EnemyBlueprint>();

        string assemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
        string blueprintFolderPath = Path.Combine(assemblyLocation, "Data", "Blueprints", "Enemies");

        if (!Directory.Exists(blueprintFolderPath))
        {
            Console.WriteLine($"[CRITICAL ERROR] Nothing found in directory: {blueprintFolderPath}");
            return loadedBlueprints; 
        }

        var blueprintFiles = Directory.GetFiles(blueprintFolderPath, "*.json");
        Console.WriteLine($"Encontrados {blueprintFiles.Length} arquivos de blueprint de inimigos.");

        foreach (var file in blueprintFiles)
        {
            try
            {
                Console.WriteLine($"Processando arquivo: {Path.GetFileName(file)}");
                var json = File.ReadAllText(file);

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var blueprint = System.Text.Json.JsonSerializer.Deserialize<EnemyBlueprint>(json, options);

                if (blueprint == null || string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    throw new InvalidDataException("O blueprint deserializado é nulo ou não possui um 'Id' válido.");
                }

                if (loadedBlueprints.ContainsKey(blueprint.Id))
                {
                    throw new InvalidDataException($"Um blueprint com o Id '{blueprint.Id}' já foi carregado. IDs devem ser únicos.");
                }

                loadedBlueprints[blueprint.Id] = blueprint;
                Console.WriteLine($" > Sucesso! Blueprint '{blueprint.Name}' (ID: {blueprint.Id}) carregado.");
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                Console.WriteLine($"[ERRO DE PARSE] Falha ao processar o arquivo '{Path.GetFileName(file)}'. O JSON está inválido. Detalhes: {jsonEx.Message}");
            }
            catch (InvalidDataException dataEx)
            {
                Console.WriteLine($"[ERRO DE DADOS] Falha ao processar o arquivo '{Path.GetFileName(file)}'. Detalhes: {dataEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO INESPERADO] Falha ao processar o arquivo '{Path.GetFileName(file)}'. Detalhes: {ex.Message}");
            }
        }

        Console.WriteLine($"Carregamento finalizado. {loadedBlueprints.Count} de {blueprintFiles.Length} blueprints foram carregados com sucesso.");
        return loadedBlueprints;
    }
}