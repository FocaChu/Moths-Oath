using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Entities;
using MothsOath.Core.Factories;
using MothsOath.Core.Models.DifficultyConfig;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.States;

public class CombatState : IGameState
{
    private readonly GameStateManager _gameManager;
    private readonly NpcFactory _npcFactory;
    private readonly StateFactory _stateFactory;

    public CombatPhase CurrentPhase { get; private set; }
    public string BiomeId { get; set; }
    public Player Player { get; private set; }
    public List<BaseCharacter> Enemies { get; private set; } = new List<BaseCharacter>();
    public List<BaseCharacter> Allies { get; private set; } = new List<BaseCharacter>();
    public List<BaseCharacter> DeadPool { get; private set; } = new List<BaseCharacter>();

    public bool CombatEnded { get; private set; } = false;
    public int TurnCount { get; private set; } = 0;
    public int EnemiesDefeatedCount { get; private set; } = 0;

    public int TotalXPReward { get; set; }
    public int TotalGoldReward { get; set; }

    public event Action OnPlayerTurnStart;
    public event Action OnEnemyTurnStart;
    public event Action OnCombatStateChanged;

    public CombatState(GameStateManager gameManager, NpcFactory npcFactory, StateFactory stateFactory, Player player)
    {
        _gameManager = gameManager;
        _npcFactory = npcFactory;
        _stateFactory = stateFactory;
        BiomeId = gameManager.Biome;
        Player = player;
    }


    public void OnEnter()
    {
        GenerateEnemies();

        var storageAllies = new List<BaseCharacter>();

        foreach (var storageAlly in Player.StorageAllies)
        {
            storageAllies.Add(storageAlly);
        }
        foreach (var storageAlly in storageAllies)
        {
            Allies.Add(storageAlly);
            Player.StorageAllies.Remove(storageAlly);
        }

        Console.WriteLine("New Combat Started!");
        Console.WriteLine($"Player: S:{Player.Stats.BaseStrength} R:{Player.Stats.BaseDefense}");

        ApplyOnCombatStartPassives();

        StartPlayerTurn();
    }

    public void OnExit()
    {
        Console.WriteLine("Saindo do estado de combate.");
    }

    public void StartCombat()
    {
        if (CurrentPhase == CombatPhase.PlayerTurn_Action || CurrentPhase == CombatPhase.EnemyTurn_Resolution)
            return;

        ApplyOnCombatStartPassives();

        StartPlayerTurn();

        OnCombatStateChanged?.Invoke();
    }

    public void Update() { }

    private void GenerateEnemies()
    {
        Enemies.Clear();
        DeadPool.Clear();
        Enemies = _npcFactory.SortEnemies(this);
    }

    private void ApplyOnCombatStartPassives()
    {
        var playerOnStartPassives = Player.PassiveEffects.OfType<ICombatStartReactor>().ToList();
        foreach (var effect in playerOnStartPassives)
        {
            effect.OnCombatStart(Player, this);
        }

        foreach (var ally in Allies)
        {
            var allyOnStartPassives = ally.PassiveEffects.OfType<ICombatStartReactor>().ToList();
            foreach (var effect in allyOnStartPassives)
            {
                effect.OnCombatStart(ally, this);
            }
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

    public void PlayCard(BaseCard card, BaseCharacter target)
    {
        if (CurrentPhase != CombatPhase.PlayerTurn_Action || Player == null) return;

        Console.WriteLine($"Jogador jogou a carta '{card.Name}' no alvo '{target.Name}'.");

        List<BaseCharacter> targets = new List<BaseCharacter> { target };

        var context = new ActionContext(Player, targets, this, card);

        Player.PlayCard(context);

        CheckForDeadAllies();
        CheckForDeadEnemies();

        OnCombatStateChanged?.Invoke();
    }

    private void CheckForDeadAllies()
    {
        var defeatedAllies = Allies.Where(a => !a.Stats.IsAlive).ToList();

        if (defeatedAllies.Any())
        {
            foreach (var defeated in defeatedAllies)
            {

                Allies.Remove(defeated);
                DeadPool.Add(defeated);
                Console.WriteLine($"Aliado '{defeated.Name}' foi derrotado!");
            }
        }
    }

    private void CheckForDeadEnemies()
    {
        var defeatedEnemies = Enemies.Where(e => !e.Stats.IsAlive).ToList();

        if (defeatedEnemies.Any())
        {
            foreach (var defeated in defeatedEnemies)
            {
                if (defeated is CharacterNPC enemy)
                {
                    TotalGoldReward += enemy.GoldReward;
                    TotalXPReward += enemy.XpReward;
                    Console.WriteLine($"Inimigo '{defeated.Name}' foi derrotado! Ouro ganho: {enemy.GoldReward}, XP ganho: {enemy.XpReward}");
                }

                this.EnemiesDefeatedCount++;

                Enemies.Remove(defeated);
                DeadPool.Add(defeated);
                Console.WriteLine($"Inimigo '{defeated.Name}' foi derrotado!");
            }
        }

        CheckForCombatEnd();
    }

    private void CheckForCombatEnd()
    {
        if (CombatEnded)
            return;

        if (Enemies.Count == 0)
        {
            CombatEnded = true;
            Console.WriteLine("VITÓRIA!");
            Player.GainXp(TotalXPReward);
            Player.Gold += TotalGoldReward;

            var playerOnCombatEndEffects = Player.StatusEffects.OfType<ICombatEndReactor>().ToList()
                                                               .Concat(Player.PassiveEffects.OfType<ICombatEndReactor>().ToList())
                                                               .OrderByDescending(m => m.Priority);

            foreach (var effect in playerOnCombatEndEffects)
            {
                effect.OnCombatEnd(this, Player);
            }

            foreach (var ally in Allies)
            {
                var allyOnCombatEndEffects = ally.StatusEffects.OfType<ICombatEndReactor>().ToList()
                                                 .Concat(ally.PassiveEffects.OfType<ICombatEndReactor>().ToList())
                                                 .OrderByDescending(m => m.Priority);

                foreach (var effect in allyOnCombatEndEffects)
                {
                    effect.OnCombatEnd(this, ally);
                }
            }

            foreach (var ally in Allies)
            {
                if (ally.Stats.IsAlive)
                {
                    ally.Clean();
                    ally.RecievePureDamage(ally.Stats.Regeneration);
                    Player.StorageAllies.Add(ally);
                }
            }

            Allies.Clear();

            Player.Clean();

            var resultState = _stateFactory.CreateCombatResultState(_gameManager, Player, TotalXPReward, TotalGoldReward, TurnCount, EnemiesDefeatedCount);
            _gameManager.TransitionToState(resultState);
        }
        else if (!Player.Stats.IsAlive)
        {
            CombatEnded = true;
            Console.WriteLine("DERROTA!");
            var nextState = _stateFactory.CreateMainMenuState(_gameManager);
            _gameManager.TransitionToState(nextState);
        }
    }

    private void StartPlayerTurn()
    {
        TurnCount++;
        ApplyStatusEffectsAtTurnStart();

        CurrentPhase = CombatPhase.PlayerTurn_Start;
        Console.WriteLine($"--- Turno do Jogador Começou HP:{Player.Stats.CurrentHealth} ---");

        OnPlayerTurnStart?.Invoke();

        Player.OnTurnStart(this);

        foreach (var enemy in Enemies)
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
            if (enemy is CharacterNPC enemyNpc)
                enemyNpc.TakeTurn(this);
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
        foreach (var ally in Allies)
        {
            ally.ActivateTurnEndEffects(this);
        }

        CheckForDeadAllies();
        CheckForDeadEnemies();

        foreach (var enemy in Enemies)
        {
            enemy.ActivateTurnEndEffects(this);
        }

        Player.ActivateTurnEndEffects(this);

        CheckForDeadAllies();
        CheckForDeadEnemies();
    }

    private void ApplyStatusEffectsAtTurnStart()
    {
        foreach (var ally in Allies)
        {
            ally.ActivateTurnStartEffects(this);
        }

        CheckForDeadAllies();
        CheckForDeadEnemies();

        foreach (var enemy in Enemies)
        {
            enemy.ActivateTurnStartEffects(this);
        }

        Player.ActivateTurnStartEffects(this);

        CheckForDeadAllies();
        CheckForDeadEnemies();
    }

    private void TickStatusEffects()
    {
        foreach (var allies in Allies)
        {
            allies.TickStatusEffects(this);
        }

        CheckForDeadAllies();
        CheckForDeadEnemies();

        foreach (var enemy in Enemies)
        {
            enemy.TickStatusEffects(this);
        }

        Player.TickStatusEffects(this);

        CheckForDeadAllies();
        CheckForDeadEnemies();
    }

    public List<BaseCharacter> BuildPlayerTeam()
    {
        var playerTeam = new List<BaseCharacter> { Player };

        for (int i = 0; i < Allies.Count; i++)
        {
            if (i % 2 == 0)
            {
                playerTeam.Insert(0, Allies[i]);
            }
            else
            {
                playerTeam.Add(Allies[i]);
            }
        }

        return playerTeam;
    }

    public List<BaseCharacter> GetAllCharacters()
    {
        List<BaseCharacter> allCharacters = new List<BaseCharacter> { Player };
        allCharacters.AddRange(Enemies);
        allCharacters.AddRange(Allies);
        return allCharacters;
    }

    public DifficultyConfig GetDifficultyConfig()
    {
        return _gameManager.DifficultyConfig;
    }

    public BaseCharacter CreateNpc(string blueprintId)
    {
        return _npcFactory.CreateNpc(blueprintId);
    }
}