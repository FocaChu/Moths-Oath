using MothsOath.Core.Behaviors;
using MothsOath.Core.Entities;
using MothsOath.Core.Entities.Archetypes;
using MothsOath.Core.Factories;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services;
using MothsOath.Core.StatusEffect.DiseaseEffect;
using MothsOath.Core.StatusEffect.DiseaseEffect.Symptoms;

namespace MothsOath.Core.States;

public class DoctorCreationState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly BlueprintLoader _blueprintLoader;
    private readonly BehaviorFactory _behaviorFactory;
    private readonly Player _basePlayer;
    private readonly List<DiseaseBlueprint> _availableDiseases;
    private readonly Dictionary<string, BaseSymptomEffect> _allSymptoms;

    public int CurrentDiseaseIndex { get; private set; } = 0;
    public DiseaseBlueprint SelectedDisease => _availableDiseases[CurrentDiseaseIndex];
    
    public BaseSymptomEffect? SelectedInitialSymptom { get; private set; }
    public string DiseaseName { get; set; } = "";
    public bool IsInitialized { get; private set; } = false;

    private DoctorCreationState(GameStateManager gameManager, BlueprintLoader blueprintLoader, BehaviorFactory behaviorFactory, Player basePlayer, List<DiseaseBlueprint> availableDiseases)
    {
        _gameManager = gameManager;
        _blueprintLoader = blueprintLoader;
        _basePlayer = basePlayer;
        _behaviorFactory = behaviorFactory;
        _availableDiseases = availableDiseases;
        
        var symptoms = GetAllSymptoms();
        _allSymptoms = symptoms
            .Where(s => !string.IsNullOrWhiteSpace(s.Id))
            .GroupBy(s => s.Id)
            .Select(g => g.First())
            .ToDictionary(s => s.Id, s => s);

        IsInitialized = true;
    }

    public static async Task<DoctorCreationState> CreateAsync(GameStateManager gameManager, BlueprintLoader blueprintLoader, BehaviorFactory behaviorFactory, Player basePlayer)
    {
        var availableDiseases = await blueprintLoader.LoadAllBlueprintsFromFilesAsync<DiseaseBlueprint>("Diseases");
        return new DoctorCreationState(gameManager, blueprintLoader, behaviorFactory, basePlayer, availableDiseases.Values.ToList());
    }

    public void NextDisease()
    {
        CurrentDiseaseIndex = (CurrentDiseaseIndex + 1) % _availableDiseases.Count;
        SelectedInitialSymptom = null; 
    }

    public void PreviousDisease()
    {
        CurrentDiseaseIndex = (CurrentDiseaseIndex - 1 + _availableDiseases.Count) % _availableDiseases.Count;
        SelectedInitialSymptom = null; 
    }

    public List<BaseSymptomEffect> GetAvailableInitialSymptoms()
    {
        var disease = SelectedDisease;
        var availableSymptoms = new List<BaseSymptomEffect>();
        
        foreach (var symptomId in disease.InitialSymptoms)
        {
            if (_allSymptoms.TryGetValue(symptomId, out var symptom))
            {
                availableSymptoms.Add(symptom);
            }
        }
        
        return availableSymptoms;
    }

    public List<BaseSymptomEffect> GetAllMutations()
    {
        var disease = SelectedDisease;
        var allMutations = new List<BaseSymptomEffect>();
        
        foreach (var symptomId in disease.ViableMutations)
        {
            if (_allSymptoms.TryGetValue(symptomId, out var symptom))
            {
                allMutations.Add(symptom);
            }
        }
        
        return allMutations;
    }

    public void SelectInitialSymptom(BaseSymptomEffect symptom)
    {
        SelectedInitialSymptom = symptom;
    }

    public void FinalizeDoctorCreation()
    {
        if (SelectedInitialSymptom == null || string.IsNullOrWhiteSpace(DiseaseName))
        {
            return; 
        }

        var diseaseBlueprint = SelectedDisease;
        var allMutations = GetAllMutations();

        var remainingMutations = allMutations.Where(s => s.Id != SelectedInitialSymptom.Id).ToList();
        
        var disease = new DiseaseEffect(
            diseaseBlueprint.Id,
            DiseaseName,
            diseaseBlueprint.Description,
            false,
            _behaviorFactory.GetBehavior(diseaseBlueprint.BehaviorId),
            new List<BaseSymptomEffect> { SelectedInitialSymptom },
            remainingMutations
        );

        var doctor = new Doctor(_basePlayer, disease);

        var combatState = _gameManager.StateFactory.CreateCombatState(_gameManager, doctor);
        _gameManager.TransitionToState(combatState);
    }

    public void GoBack()
    {
        var creationState = _gameManager.StateFactory.CreatePlayerCreationState(_gameManager);
        _gameManager.TransitionToState(creationState);
    }

    private List<BaseSymptomEffect> GetAllSymptoms()
    {
        var symptoms = new List<BaseSymptomEffect>();
        var assembly = typeof(DoctorCreationState).Assembly;
        var symptomTypes = assembly.GetTypes()
            .Where(t => typeof(BaseSymptomEffect).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in symptomTypes)
        {
            try
            {
                object? created = assembly.CreateInstance(type.FullName ?? string.Empty);

                if (created == null)
                {
                    var defaultCtor = type.GetConstructor(Type.EmptyTypes);
                    if (defaultCtor != null)
                    {
                        created = defaultCtor.Invoke(null);
                    }
                }

                if (created is BaseSymptomEffect instance && !string.IsNullOrWhiteSpace(instance.Id))
                {
                    symptoms.Add(instance);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Não foi possível instanciar '{type.FullName}': {ex.Message}");
            }
        }

        return symptoms;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Update()
    {
    }
}

