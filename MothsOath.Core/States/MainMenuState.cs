using MothsOath.Core.Entities;
using MothsOath.Core.Factories;

namespace MothsOath.Core.States;

public class MainMenuState : IGameState
{
    private readonly GameManager _gameManager;
    private readonly StateFactory _stateFactory;

    public MainMenuState(GameManager gameManager, StateFactory stateFactory)
    {
        _gameManager = gameManager;
        _stateFactory = stateFactory;
    }

    public void StartNewGame()
    {
        Console.WriteLine("Iniciando novo jogo...");
        var player = new Player();

        var nextState = _stateFactory.CreateCombatState(_gameManager, player);

        _gameManager.TransitionToState(nextState);
    }

    public void OnEnter() { }
    public void OnExit() {  }
    public void Update() { }
}
