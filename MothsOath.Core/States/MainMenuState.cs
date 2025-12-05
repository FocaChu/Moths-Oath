using MothsOath.Core.Entities;
using MothsOath.Core.Factories;

namespace MothsOath.Core.States;

public class MainMenuState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly StateFactory _stateFactory;
    private readonly PlayerFactory _playerFactory;

    public MainMenuState(GameStateManager gameManager, PlayerFactory playerFactory, StateFactory stateFactory)
    {
        _gameManager = gameManager;
        _playerFactory = playerFactory;
        _stateFactory = stateFactory;
    }

    public void StartNewGame()
    {
        Console.WriteLine("Iniciando novo jogo...");

        var creationState = _gameManager.StateFactory.CreatePlayerCreationState(_gameManager);
        _gameManager.TransitionToState(creationState);
    }
    

    public void OnEnter() { }
    public void OnExit() {  }
    public void Update() { }
}
