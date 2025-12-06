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

    public DoctorCreationState(GameStateManager gameManager, BlueprintLoader blueprintLoader, BehaviorFactory behaviorFactory, Player basePlayer)
    {
        _gameManager = gameManager;
        _blueprintLoader = blueprintLoader;
        _basePlayer = basePlayer;
        _behaviorFactory = behaviorFactory;
        _availableDiseases = _blueprintLoader.LoadAllBlueprintsFromFiles<DiseaseBlueprint>("Diseases").Values.ToList();
        
        var symptoms = GetAllSymptoms();
        _allSymptoms = symptoms
            .Where(s => !string.IsNullOrWhiteSpace(s.Id))
            .GroupBy(s => s.Id)
            .Select(g => g.First())
            .ToDictionary(s => s.Id, s => s);
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
                var defaultCtor = type.GetConstructor(Type.EmptyTypes);
                BaseSymptomEffect? instance = null;

                if (defaultCtor != null)
                {
                    instance = defaultCtor.Invoke(null) as BaseSymptomEffect;
                }
                else
                {
                    var ctor = type.GetConstructor(new[] { typeof(string), typeof(string), typeof(string) });
                    if (ctor != null)
                    {
                        var className = type.Name;
                        string id, name, description;
                        if (className == "NecrosisSymptomEffect")
                        {
                            id = "necrosis_symptom";
                            name = "Necrosis";
                            description = "Causes tissue death and decay.";
                        }
                        else if (className == "HemorrhageSymptomEffect")
                        {
                            id = "hemorrhage_symptom";
                            name = "Hemorragia";
                            description = "Causa sangramento.";
                        }
                        else
                        {
                            id = className.Replace("SymptomEffect", "").ToLower() + "_symptom";
                            name = className.Replace("SymptomEffect", "");
                            description = $"Sintoma {name}";
                        }
                        instance = ctor.Invoke(new object[] { id, name, description }) as BaseSymptomEffect;
                    }
                }

                if (instance != null && !string.IsNullOrWhiteSpace(instance.Id))
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

