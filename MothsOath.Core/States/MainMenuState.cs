using MothsOath.Core.Entities;
using MothsOath.Core.Factories;

namespace MothsOath.Core.States;

public class MainMenuState : IGameState
{
    private readonly GameManager _gameManager;

    public MainMenuState(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void StartNewGame()
    {
        Console.WriteLine("Iniciando novo jogo...");
        var player = new Player();
        var abilityFactory = new AbilityFactory(); 
        var enemyFactory = new EnemyFactory(abilityFactory);

        _gameManager.TransitionToState(new CombatState(_gameManager, enemyFactory, player));
    }

    public void OnEnter() { }
    public void OnExit() {  }
    public void Update() { }
}
