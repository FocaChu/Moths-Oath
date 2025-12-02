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

        var player = _playerFactory.CreatePlayer("Jorge", "human", "doctor");

        var nextState = _stateFactory.CreateCombatState(_gameManager, player);

        _gameManager.TransitionToState(nextState);
    }

    public void OnEnter() { }
    public void OnExit() {  }
    public void Update() { }
}
