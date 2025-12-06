using MothsOath.Core.Entities;
using MothsOath.Core.Entities.Archetypes;
using MothsOath.Core.Factories;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services;
using MothsOath.Core.StatusEffect.DiseaseEffect;
using MothsOath.Core.StatusEffect.DiseaseEffect.Symptoms;

namespace MothsOath.Core.States;

public class PlayerCreationState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly PlayerFactory _playerFactory;
    private readonly BlueprintLoader _blueprintLoader;
    private readonly List<RaceBlueprint> _availableRaces;
    private readonly List<ArchetypeBlueprint> _availableArchetypes ;

    public int CurrentRaceIndex { get; private set; } = 0;
    public RaceBlueprint SelectedRace => _availableRaces[CurrentRaceIndex];

    public int CurrentArchetypeIndex { get; private set; } = 0;
    public ArchetypeBlueprint SelectedArchetype => _availableArchetypes[CurrentArchetypeIndex];

    public string PlayerName { get; set; } = "Aventureiro";

    public PlayerCreationState(GameStateManager gameManager, PlayerFactory playerFactory, BlueprintLoader blueprintLoader)
    {
        _gameManager = gameManager;
        _playerFactory = playerFactory;
        _blueprintLoader = blueprintLoader;

        _availableRaces = _playerFactory.GetAllRaceBlueprints();
        _availableArchetypes = _playerFactory.GetAllArchetypeBlueprints();
    }

    public void NextRace()
    {
        CurrentRaceIndex = (CurrentRaceIndex + 1) % _availableRaces.Count;
    }

    public void PreviousRace()
    {
        CurrentRaceIndex = (CurrentRaceIndex - 1 + _availableRaces.Count) % _availableRaces.Count;
    }

    public void NextClass()
    {
        CurrentArchetypeIndex = (CurrentArchetypeIndex + 1) % _availableArchetypes.Count;
    }

    public void PreviousClass()
    {
        CurrentArchetypeIndex = (CurrentArchetypeIndex - 1 + _availableArchetypes.Count) % _availableArchetypes.Count;
    }

    public void FinalizeCreation()
    {
        var player = _playerFactory.CreatePlayer(PlayerName, SelectedRace.Id, SelectedArchetype.Id);

        if (player.Archetype == "Doutor")
        {
            var doctorCreationState = _gameManager.StateFactory.CreateDoctorCreationState(_gameManager, player);
            _gameManager.TransitionToState(doctorCreationState);
            return;
        }

        var combatState = _gameManager.StateFactory.CreateCombatState(_gameManager, player);
        _gameManager.TransitionToState(combatState);
    }

    public void GoBackToMenu()
    {
        var menuState = _gameManager.StateFactory.CreateMainMenuState(_gameManager);
        _gameManager.TransitionToState(menuState);
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
