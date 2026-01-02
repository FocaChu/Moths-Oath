using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services.Repositories;

namespace MothsOath.Core.Services;

/// <summary>
/// Service responsible for initializing all game systems during loading screen.
/// Preloads all blueprints and performs necessary setup.
/// </summary>
public class GameInitializationService
{
    private readonly IBlueprintRepository<NpcBlueprint> _npcRepo;
    private readonly IBlueprintRepository<ArchetypeBlueprint> _archetypeRepo;
    private readonly IBlueprintRepository<RaceBlueprint> _raceRepo;
    private readonly IBlueprintRepository<DiseaseBlueprint> _diseaseRepo;
    
    public event Action<string>? OnLoadingStatusChanged;
    public event Action<float>? OnLoadingProgressChanged;
    
    public GameInitializationService(
        IBlueprintRepository<NpcBlueprint> npcRepo,
        IBlueprintRepository<ArchetypeBlueprint> archetypeRepo,
        IBlueprintRepository<RaceBlueprint> raceRepo,
        IBlueprintRepository<DiseaseBlueprint> diseaseRepo)
    {
        _npcRepo = npcRepo ?? throw new ArgumentNullException(nameof(npcRepo));
        _archetypeRepo = archetypeRepo ?? throw new ArgumentNullException(nameof(archetypeRepo));
        _raceRepo = raceRepo ?? throw new ArgumentNullException(nameof(raceRepo));
        _diseaseRepo = diseaseRepo ?? throw new ArgumentNullException(nameof(diseaseRepo));
    }
    
    /// <summary>
    /// Initializes all game systems.
    /// This should be called during the loading screen.
    /// </summary>
    public async Task InitializeAsync()
    {
        Console.WriteLine("[INIT] Starting game initialization...");
        
        var tasks = new[]
        {
            LoadWithProgress("NPCs", _npcRepo.PreloadAsync(), 0.0f, 0.25f),
            LoadWithProgress("Archetypes", _archetypeRepo.PreloadAsync(), 0.25f, 0.50f),
            LoadWithProgress("Races", _raceRepo.PreloadAsync(), 0.50f, 0.75f),
            LoadWithProgress("Diseases", _diseaseRepo.PreloadAsync(), 0.75f, 1.0f)
        };
        
        try
        {
            await Task.WhenAll(tasks);
            
            OnLoadingStatusChanged?.Invoke("Initialization complete!");
            OnLoadingProgressChanged?.Invoke(1.0f);
            
            Console.WriteLine("[INIT] Game initialization complete");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Game initialization failed: {ex.Message}");
            throw;
        }
    }
    
    private async Task LoadWithProgress(string name, Task loadTask, float startProgress, float endProgress)
    {
        OnLoadingStatusChanged?.Invoke($"Loading {name}...");
        OnLoadingProgressChanged?.Invoke(startProgress);
        
        await loadTask;
        
        OnLoadingProgressChanged?.Invoke(endProgress);
    }
}
