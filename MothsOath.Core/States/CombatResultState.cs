using MothsOath.Core.Entities;
using MothsOath.Core.Factories;

namespace MothsOath.Core.States;

public class CombatResultState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly StateFactory _stateFactory;

    public Player Player { get; }
    public int TotalXp { get; }
    public int TotalGold { get; }
    public int TurnCount { get; }
    public int EnemiesDefeated { get; }

    public CombatResultState(GameStateManager gameManager, StateFactory stateFactory, Player player, int totalXp, int totalGold, int turnCount, int enemiesDefeated)
    {
        _gameManager = gameManager;
        _stateFactory = stateFactory;
        Player = player;
        TotalXp = totalXp;
        TotalGold = totalGold;
        TurnCount = turnCount;
        EnemiesDefeated = enemiesDefeated;
    }

    public void OnEnter()
    {
    }

    public void OnExit() { }

    public void Update() { }

    public void ConfirmAndContinue()
    {
        var nextCombat = _stateFactory.CreateCombatState(_gameManager, Player);
        _gameManager.TransitionToState(nextCombat);
    }
}
