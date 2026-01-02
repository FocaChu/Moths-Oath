using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services.Repositories;

namespace MothsOath.Core.Services;

/// <summary>
/// Adapter that bridges the new repository system with the old BlueprintCache API.
/// This allows existing code to keep working while new code can use repositories directly.
/// </summary>
public class BlueprintCacheAdapter
{
    private readonly IBlueprintRepository<NpcBlueprint> _npcRepo;
    private readonly IBlueprintRepository<ArchetypeBlueprint> _archetypeRepo;
    private readonly IBlueprintRepository<RaceBlueprint> _raceRepo;
    private readonly IBlueprintRepository<DiseaseBlueprint> _diseaseRepo;
    
    public BlueprintCacheAdapter(
        IBlueprintRepository<NpcBlueprint> npcRepo,
        IBlueprintRepository<ArchetypeBlueprint> archetypeRepo,
        IBlueprintRepository<RaceBlueprint> raceRepo,
        IBlueprintRepository<DiseaseBlueprint> diseaseRepo)
    {
        _npcRepo = npcRepo;
        _archetypeRepo = archetypeRepo;
        _raceRepo = raceRepo;
        _diseaseRepo = diseaseRepo;
    }
    
    /// <summary>
    /// Gets all NPC blueprints as a dictionary (compatible with old API).
    /// </summary>
    public Dictionary<string, NpcBlueprint> GetNpcs()
    {
        return new Dictionary<string, NpcBlueprint>(_npcRepo.All);
    }
    
    /// <summary>
    /// Gets all archetype blueprints as a dictionary (compatible with old API).
    /// </summary>
    public Dictionary<string, ArchetypeBlueprint> GetArchetypes()
    {
        return new Dictionary<string, ArchetypeBlueprint>(_archetypeRepo.All);
    }
    
    /// <summary>
    /// Gets all race blueprints as a dictionary (compatible with old API).
    /// </summary>
    public Dictionary<string, RaceBlueprint> GetRaces()
    {
        return new Dictionary<string, RaceBlueprint>(_raceRepo.All);
    }
    
    /// <summary>
    /// Gets all disease blueprints as a dictionary (compatible with old API).
    /// </summary>
    public Dictionary<string, DiseaseBlueprint> GetDiseases()
    {
        return new Dictionary<string, DiseaseBlueprint>(_diseaseRepo.All);
    }
}
