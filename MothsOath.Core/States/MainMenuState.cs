using MothsOath.Core.Entities;
using MothsOath.Core.Factories;

namespace MothsOath.Core.States;

public class MainMenuState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly StateFactory _stateFactory;

    public MainMenuState(GameStateManager gameManager, StateFactory stateFactory)
    {
        _gameManager = gameManager;
        _stateFactory = stateFactory;
    }

    public void StartNewGame()
    {
        Console.WriteLine("Iniciando novo jogo...");
        var player = new Player { Name = "Jorge", MaxHealth = 100, CurrentHealth = 100, BaseStrength = 10};

        var nextState = _stateFactory.CreateCombatState(_gameManager, player);

        _gameManager.TransitionToState(nextState);
    }

    public void OnEnter() { }
    public void OnExit() {  }
    public void Update() { }
}
