using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.EffectInterfaces.Turn;
using MothsOath.Core.Entities;
using MothsOath.Core.Factories;
using MothsOath.Core.Models.DifficultyConfig;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.States;

public class CombatState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly EnemyFactory _enemyFactory;
    private readonly StateFactory _stateFactory;

    public CombatPhase CurrentPhase { get; private set; }
    public string BiomeId { get; set; }
    public Player Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    public List<Enemy> DeadEnemies { get; private set; } = new List<Enemy>();

    public int TurnCount { get; private set; } = 0;
    public int EnemiesDefeatedCount { get; private set; } = 0;

    public int TotalXPReward { get; set; }
    public int TotalGoldReward { get; set; }

    public event Action OnPlayerTurnStart;
    public event Action OnEnemyTurnStart;
    public event Action OnCombatStateChanged;

    public CombatState(GameStateManager gameManager, EnemyFactory enemyFactory, StateFactory stateFactory, Player player)
    {
        _gameManager = gameManager;
        _enemyFactory = enemyFactory;
        _stateFactory = stateFactory;
        BiomeId = gameManager.Biome;
        Player = player;
    }


    public void OnEnter()
    {
        GenerateEnemies();

        Console.WriteLine("New Combat Started!");
        Console.WriteLine($"Player: S:{Player.Stats.BaseStrength} R:{Player.Stats.BaseDefense}");

        ApplyOnCombatStartPassives();

        StartPlayerTurn();
    }

    public void OnExit()
    {
        Console.WriteLine("Saindo do estado de combate.");
    }

    public void Update() { }

    private void GenerateEnemies()
    {
        Enemies.Clear();
        DeadEnemies.Clear();
        Enemies = _enemyFactory.SortEnemies(this);
    }

    private void ApplyOnCombatStartPassives()
    {
        var playerOnStartPassives = Player.PassiveEffects.OfType<ICombatStartReactor>().ToList();
        foreach (var effect in playerOnStartPassives)
        {
            effect.OnCombatStart(Player, this);
        }

        foreach (var enemy in Enemies)
        {
            var enemyOnStartPassives = enemy.PassiveEffects.OfType<ICombatStartReactor>().ToList();
            foreach (var effect in enemyOnStartPassives)
            {
                effect.OnCombatStart(enemy, this);
            }
        }
    }

    public void PlayCard(BaseCard card, Character target)
    {
        if (CurrentPhase != CombatPhase.PlayerTurn_Action || Player == null) return;

        Console.WriteLine($"Jogador jogou a carta '{card.Name}' no alvo '{target.Name}'.");

        List<Character> targets = new List<Character> { target };

        var context = new ActionContext(Player, targets, this, card);

        Player.PlayCard(context);

        CheckForDeadEnemies();

        OnCombatStateChanged?.Invoke();
    }

    private void CheckForDeadEnemies()
    {
        var defeatedEnemies = Enemies.Where(e => !e.Stats.IsAlive).ToList();

        if (defeatedEnemies.Any())
        {
            foreach (var defeated in defeatedEnemies)
            {
                this.TotalGoldReward += defeated.BaseGold;
                this.TotalXPReward += defeated.BaseXp;
                this.EnemiesDefeatedCount++;

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
            Player.GainXp(TotalXPReward);
            Player.Gold += TotalGoldReward;

            var resultState = _stateFactory.CreateCombatResultState(_gameManager, Player, TotalXPReward, TotalGoldReward, TurnCount, EnemiesDefeatedCount);
            _gameManager.TransitionToState(resultState);

            this.TotalXPReward = 0;
            this.TotalGoldReward = 0;
        }
        else if (!Player.Stats.IsAlive)
        {
            Console.WriteLine("DERROTA!");
            var nextState = _stateFactory.CreateMainMenuState(_gameManager);
            _gameManager.TransitionToState(nextState);
        }
    }

    private void StartPlayerTurn()
    {
        TurnCount++;
        ApplyStatusEffectsAtTurnStart();
        CheckFadingStatusEffects();

        CurrentPhase = CombatPhase.PlayerTurn_Start;
        Console.WriteLine($"--- Turno do Jogador Começou HP:{Player.Stats.CurrentHealth} ---");

        OnPlayerTurnStart?.Invoke();

        Player.OnTurnStart(this);

        foreach(var enemy in Enemies)
        {
            enemy.Restore();
        }

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

        ApplyStatusEffectsAtTurnEnd();

        TickStatusEffects();

        StartPlayerTurn();
    }

    private void ApplyStatusEffectsAtTurnEnd()
    {
        foreach (var enemy in Enemies)
        {
            var effects = enemy.StatusEffects.OfType<ITurnEndReactor>().ToList();
            foreach (var effect in effects)
            {
                effect.OnTurnEnd(enemy, this);
            }

        }

        foreach (var effect in Player.StatusEffects.OfType<ITurnEndReactor>())
        {
            effect.OnTurnEnd(Player, this);
        }

        CheckForDeadEnemies();
    }

    private void ApplyStatusEffectsAtTurnStart()
    {
        foreach (var enemy in Enemies)
        {
            var effects = enemy.StatusEffects.OfType<ITurnStartReactor>().ToList();
            foreach (var effect in effects)
            {
                effect.OnTurnStart(enemy, this);
            }

        }

        foreach (var effect in Player.StatusEffects.OfType<ITurnStartReactor>())
        {
            effect.OnTurnStart(Player, this);
        }

        CheckForDeadEnemies();
    }

    private void TickStatusEffects()
    {
        foreach (var enemy in Enemies)
        {
            enemy.TickStatusEffects();
        }

        Player.TickStatusEffects();

        CheckForDeadEnemies();
    }

    private void CheckFadingStatusEffects()
    {
        foreach (var character in GetAllCharacters())
        {
            character.ClearFadingStatusEffects();
        }
    }

    public List<Character> GetAllCharacters()
    {
        List<Character> allCharacters = new List<Character> { Player };
        allCharacters.AddRange(Enemies);
        return allCharacters;
    }

    public DifficultyConfig GetDifficultyConfig()
    {
        return _gameManager.DifficultyConfig;
    }
}