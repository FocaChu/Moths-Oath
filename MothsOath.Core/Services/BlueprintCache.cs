using MothsOath.Core.Models.Blueprints;
using System.Text.Json;

namespace MothsOath.Core.Services;

public class BlueprintCache
{
    private readonly BlueprintLoader _loader;
    private Dictionary<string, RaceBlueprint>? _races;
    private Dictionary<string, ArchetypeBlueprint>? _archetypes;
    private Dictionary<string, JsonElement>? _cards;
    private Dictionary<string, NpcBlueprint>? _npcs;
    private Dictionary<string, JsonElement>? _tags;

    public BlueprintCache(BlueprintLoader loader)
    {
        _loader = loader;
    }

    public async Task InitializeAsync()
    {
        _races = await _loader.LoadAllBlueprintsFromFilesAsync<RaceBlueprint>("Races");
        _archetypes = await _loader.LoadAllBlueprintsFromFilesAsync<ArchetypeBlueprint>("Archetypes");
        _cards = await _loader.LoadAllRawBlueprintsAsync("Cards");
        _npcs = await _loader.LoadAllBlueprintsFromFilesAsync<NpcBlueprint>("NPCs");
        _tags = await _loader.LoadAllRawBlueprintsAsync("Tags");
    }

    public Dictionary<string, RaceBlueprint> GetRaces()
    {
        return _races ?? throw new InvalidOperationException("Blueprints não foram inicializados. Chame InitializeAsync primeiro.");
    }

    public Dictionary<string, ArchetypeBlueprint> GetArchetypes()
    {
        return _archetypes ?? throw new InvalidOperationException("Blueprints não foram inicializados. Chame InitializeAsync primeiro.");
    }

    public Dictionary<string, JsonElement> GetCards()
    {
        return _cards ?? throw new InvalidOperationException("Blueprints não foram inicializados. Chame InitializeAsync primeiro.");
    }

    public Dictionary<string, NpcBlueprint> GetNpcs()
    {
        return _npcs ?? throw new InvalidOperationException("Blueprints não foram inicializados. Chame InitializeAsync primeiro.");
    }

    public Dictionary<string, JsonElement> GetTags()
    {
        return _tags ?? throw new InvalidOperationException("Blueprints não foram inicializados. Chame InitializeAsync primeiro.");
    }
}

