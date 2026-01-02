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
    public List<BaseCharacter> PlayerTeam { get; private set; } = new List<BaseCharacter>();
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

        BuildPlayerTeam();

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

        ClearFadedEffects();

        OnCombatStateChanged?.Invoke();
    }

    private void CheckForDeadAllies()
    {
        var defeatedAllies = Allies.Where(a => !a.Stats.IsAlive).ToList();

        if (!defeatedAllies.Any())
            return;

        // Process defeated allies first, then modify collections
        foreach (var defeated in defeatedAllies)
        {
            DeadPool.Add(defeated);
            Console.WriteLine($"Aliado '{defeated.Name}' foi derrotado!");
        }

        // Now safely remove from collections
        Allies.RemoveAll(a => !a.Stats.IsAlive);
        PlayerTeam.RemoveAll(a => !a.Stats.IsAlive);
    }

    private void CheckForDeadEnemies()
    {
        var defeatedEnemies = Enemies.Where(e => !e.Stats.IsAlive).ToList();

        if (!defeatedEnemies.Any())
            return;

        // Process defeated enemies and calculate rewards first
        foreach (var defeated in defeatedEnemies)
        {
            if (defeated is CharacterNPC enemy)
            {
                TotalGoldReward += enemy.GoldReward;
                TotalXPReward += enemy.XpReward;
                Console.WriteLine($"Inimigo '{defeated.Name}' foi derrotado! Ouro ganho: {enemy.GoldReward}, XP ganho: {enemy.XpReward}");
            }

            this.EnemiesDefeatedCount++;
            DeadPool.Add(defeated);
            Console.WriteLine($"Inimigo '{defeated.Name}' foi derrotado!");
        }

        // Now safely remove from collection
        Enemies.RemoveAll(e => !e.Stats.IsAlive);

        CheckForCombatEnd();
    }

    private void CheckForCombatEnd()
    {
        if (CombatEnded)
            return;

        if (!Player.Stats.IsAlive)
        {
            HandleDefeat();
        }
        else if (Enemies.Count == 0)
        {
            HandleVictory();
        }
    }

    private void HandleVictory()
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
                ally.ReceivePureDamage(ally.Stats.Regeneration);
                Player.StorageAllies.Add(ally);
            }
        }

        Allies.Clear();

        Player.Clean();

        var resultState = _stateFactory.CreateCombatResultState(_gameManager, Player, TotalXPReward, TotalGoldReward, TurnCount, EnemiesDefeatedCount);
        _gameManager.TransitionToState(resultState);
    }

    private void HandleDefeat()
    {
        CombatEnded = true;
        Console.WriteLine("DERROTA!");
        var nextState = _stateFactory.CreateMainMenuState(_gameManager);
        _gameManager.TransitionToState(nextState);
    }

    private void StartPlayerTurn()
    {
        TurnCount++;
        ApplyStatusEffectsAtTurnStart();

        CurrentPhase = CombatPhase.PlayerTurn_Start;
        Console.WriteLine($"--- Turno do Jogador Começou HP:{Player.Stats.CurrentHealth} ---");

        OnPlayerTurnStart?.Invoke();

        Player.OnTurnStart(this);

        foreach (var ally in Allies)
        {
            ally.Restore();
        }

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

        ExecuteAlliesTurns();
        ExecuteEnemyTurns();
        EndTurn();
    }

    private void ExecuteAlliesTurns()
    {
        CurrentPhase = CombatPhase.EnemyTurn_Resolution;
        var allies = new List<BaseCharacter>(Allies);

        foreach (var ally in allies)
        {
            if (CombatEnded)
                return;

            if (ally is CharacterNPC allyNpc)
                allyNpc.TakeTurn(this);

            ClearFadedEffects();
        }
    }

    private void ExecuteEnemyTurns()
    {
        CurrentPhase = CombatPhase.EnemyTurn_Resolution;
        var enemies = new List<BaseCharacter>(Enemies);

        foreach (var enemy in enemies)
        {
            if (CombatEnded)
                return;

            if (enemy is CharacterNPC enemyNpc)
                enemyNpc.TakeTurn(this);

            ClearFadedEffects();
        }
    }

    private void EndTurn()
    {
        CurrentPhase = CombatPhase.TurnEnd;
        Console.WriteLine("--- Fim do Turno ---");

        ApplyStatusEffectsAtTurnEnd();

        ClearFadedEffects();

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

        ClearFadedEffects();
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

    private void ClearFadedEffects()
    {
        foreach (var ally in Allies)
        {
            ally.ClearFadingStatusEffects(this);
        }

        CheckForDeadAllies();
        CheckForDeadEnemies();

        foreach (var enemy in Enemies)
        {
            enemy.ClearFadingStatusEffects(this);
        }

        Player.ClearFadingStatusEffects(this);

        CheckForDeadAllies();
        CheckForDeadEnemies();
    }

    public void AddAlly(BaseCharacter ally)
    {
        if (PlayerTeam.Count >= 5)
        {
            Console.WriteLine("A equipe está cheia!");
            return;
        }

        Allies.Add(ally);

        int playerIndex = PlayerTeam.FindIndex(c => c is Player);

        if (playerIndex == -1)
        {
            PlayerTeam.Add(ally);
            return;
        }

        int alliesToTheLeft = playerIndex;
        int alliesToTheRight = PlayerTeam.Count - 1 - playerIndex;

        if (alliesToTheRight <= alliesToTheLeft)
        {
            PlayerTeam.Add(ally);
            Console.WriteLine($"Adicionado ao FINAL (Direita tinha {alliesToTheRight} aliados, Esquerda tinha {alliesToTheLeft})");
        }
        else
        {
            PlayerTeam.Insert(0, ally);
            Console.WriteLine($"Adicionado ao INÍCIO (Esquerda tinha {alliesToTheLeft} aliados, Direita tinha {alliesToTheRight})");
        }

        Console.WriteLine($"Aliado '{ally.Name}' adicionado. Equipe atual: {string.Join(", ", PlayerTeam.Select(a => a.Name))}");
    }

    public void BuildPlayerTeam()
    {
        PlayerTeam.Clear();
        PlayerTeam.Add(Player);

        for (int i = 0; i < Allies.Count; i++)
        {
            if (i % 2 == 0)
            {
                PlayerTeam.Insert(0, Allies[i]);
            }
            else
            {
                PlayerTeam.Add(Allies[i]);
            }
        }
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