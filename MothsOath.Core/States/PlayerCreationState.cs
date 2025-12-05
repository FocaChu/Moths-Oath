using MothsOath.Core.Entities.Archetypes;
using MothsOath.Core.Factories;
using MothsOath.Core.Models.Blueprints;

namespace MothsOath.Core.States;

public class PlayerCreationState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly PlayerFactory _playerFactory;
    private readonly List<RaceBlueprint> _availableRaces;
    private readonly List<ArchetypeBlueprint> _availableArchetypes ;

    public int CurrentRaceIndex { get; private set; } = 0;
    public RaceBlueprint SelectedRace => _availableRaces[CurrentRaceIndex];

    public int CurrentArchetypeIndex { get; private set; } = 0;
    public ArchetypeBlueprint SelectedArchetype => _availableArchetypes[CurrentArchetypeIndex];

    public string PlayerName { get; set; } = "Aventureiro";

    public string DiseaseName { get; set; } = "Peste";

    public PlayerCreationState(GameStateManager gameManager, PlayerFactory playerFactory)
    {
        _gameManager = gameManager;
        _playerFactory = playerFactory;

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

        if (player is Doctor doctor)
        {
            doctor.Lab.DisieaseName = this.DiseaseName;
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
