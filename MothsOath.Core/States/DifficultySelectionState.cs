using MothsOath.Core.Factories;
using MothsOath.Core.Models.DifficultyConfig;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.States;

public class DifficultySelectionState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly StateFactory _stateFactory;

    public GameDifficulty SelectedDifficulty { get; private set; } = GameDifficulty.Normal;

    public DifficultyConfig Config { get; private set; } = new DifficultyConfig();

    public DifficultySelectionState(GameStateManager gameManager, StateFactory stateFactory)
    {
        _gameManager = gameManager;
        _stateFactory = stateFactory;
        Config = _gameManager.DifficultyConfig ?? new DifficultyConfig();
    }

    public void SetDifficulty(GameDifficulty difficulty)
    {
        SelectedDifficulty = difficulty;
    }

    public void Confirm()
    {
        var resolved = Resolvers.DifficultyResolver.ResolveDifficultyConfig(SelectedDifficulty);
        _gameManager.Difficulty = SelectedDifficulty;
        _gameManager.DifficultyConfig = resolved;

        var creationState = _stateFactory.CreatePlayerCreationState(_gameManager);
        _gameManager.TransitionToState(creationState);
    }

    public void OnEnter() { }
    public void OnExit() { }
    public void Update() { }
}
