using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Factories;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.StatusEffect.Interfaces;

namespace MothsOath.Core.States;

public class CombatState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly EnemyFactory _enemyFactory;
    private readonly StateFactory _stateFactory;

    public CombatPhase CurrentPhase { get; private set; }
    public Player Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    public List<Enemy> DeadEnemies { get; private set; } = new List<Enemy>();

    public event Action OnPlayerTurnStart;
    public event Action OnEnemyTurnStart;

    public CombatState(GameStateManager gameManager, EnemyFactory enemyFactory, StateFactory stateFactory, Player player)
    {
        _gameManager = gameManager;
        _enemyFactory = enemyFactory;
        _stateFactory = stateFactory;
        Player = player;
    }


    public void OnEnter()
    {
        Enemies.Clear();
        DeadEnemies.Clear();
        Enemies.Add(_enemyFactory.CreateEnemy("skeleton_warrior"));
        Enemies.Add(_enemyFactory.CreateEnemy("skeleton_warrior"));

        Console.WriteLine("New Combat Started!");

        StartPlayerTurn();
    }

    public void OnExit()
    {
        Console.WriteLine("Saindo do estado de combate.");
    }

    public void Update() { }

    public void PlayCard(BaseCard card, Character target)
    {
        if (CurrentPhase != CombatPhase.PlayerTurn_Action || Player == null) return;

        Console.WriteLine($"Jogador jogou a carta '{card.Name}' no alvo '{target.Name}'.");

        List<Character> targets = new List<Character> { target };

        var context = new ActionContext(Player, targets, this, card);

        Player.PlayCard(context);

        CheckForDeadEnemies();
    }

    private void CheckForDeadEnemies()
    {
        var defeatedEnemies = Enemies.Where(e => !e.IsAlive).ToList();

        if (defeatedEnemies.Any())
        {
            foreach (var defeated in defeatedEnemies)
            {
                Enemies.Remove(defeated);
                DeadEnemies.Add(defeated);
                Console.WriteLine($"Inimigo '{defeated.Name}' foi derrotado!");
            }
        }

        CheckForCombatEnd();
    }

    private void CheckForCombatEnd()
    {
        if (Enemies.Count == 0)
        {
            Console.WriteLine("VITÓRIA!");
            var nextState = _stateFactory.CreateMainMenuState(_gameManager);
            _gameManager.TransitionToState(nextState);
        }
        else if (!Player.IsAlive)
        {
            Console.WriteLine("DERROTA!");
            var nextState = _stateFactory.CreateMainMenuState(_gameManager);
            _gameManager.TransitionToState(nextState);
        }
    }

    private void StartPlayerTurn()
    {
        CurrentPhase = CombatPhase.PlayerTurn_Start;
        Console.WriteLine("--- Turno do Jogador Começou ---");

        OnPlayerTurnStart?.Invoke();

        if (Player.CardsByTurn > Player.Deck.Count)
        {
            Player.DrawCards(Player.Deck.Count > 0 ? Player.Deck.Count : 1);
        }
        else
        {
            Player.DrawCards(Player.CardsByTurn);
        }
        Player.DrawCards(Player.CardsByTurn);

        CurrentPhase = CombatPhase.PlayerTurn_Action;
    }

    public void EndPlayerTurn()
    {
        CurrentPhase = CombatPhase.EnemyTurn_Start;
        Console.WriteLine("--- Turno do Inimigo Começou ---");
        OnEnemyTurnStart?.Invoke();

        ExecuteEnemyTurns();
    }

    private void ExecuteEnemyTurns()
    {
        CurrentPhase = CombatPhase.EnemyTurn_Resolution;

        foreach (var enemy in Enemies)
        {
            enemy.TakeTurn(this);
        }

        EndTurn();
    }

    private void EndTurn()
    {
        CurrentPhase = CombatPhase.TurnEnd;
        Console.WriteLine("--- Fim do Turno ---");

        foreach (var enemy in Enemies)
        {
            if (enemy.StatusEffects.Any())
            {
                var effects = enemy.StatusEffects.OfType<ITurnBasedEffect>().ToList();
                foreach (var effect in effects)
                {
                    effect.OnTurnEnd(enemy, this);
                }
            }

            CheckForDeadEnemies();

            StartPlayerTurn();
        }
    }
}