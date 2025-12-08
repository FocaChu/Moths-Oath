using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services;
using MothsOath.Core.States;

namespace MothsOath.Core.Factories;

public class EnemyFactory
{
    private readonly Dictionary<string, EnemyBlueprint> _enemyBlueprints;
    private readonly ActionFactory _abilityFactory;
    private readonly BehaviorFactory _behaviorFactory;
    private readonly PassiveEffectFactory _passiveEffectFactory;

    public EnemyFactory(ActionFactory abilityFactory, BehaviorFactory behaviorFactory , PassiveEffectFactory passiveEffectFactory, BlueprintLoader blueprintLoader)
    {
        _abilityFactory = abilityFactory;
        _behaviorFactory = behaviorFactory;
        _passiveEffectFactory = passiveEffectFactory;
        _enemyBlueprints = blueprintLoader.LoadAllBlueprintsFromFiles<EnemyBlueprint>("Enemies");
    }

    public Enemy CreateEnemy(string blueprintId)
    {
        if (!_enemyBlueprints.TryGetValue(blueprintId, out var blueprint))
        {
            throw new Exception($"Blueprint de inimigo '{blueprintId}' não encontrado!");
        }

        var stats = new Stats
        {
            MaxHealth = blueprint.MaxHealth,
            CurrentHealth = blueprint.MaxHealth,
            BaseStrength = blueprint.BaseStrength,
            BaseKnowledge = blueprint.BaseKnowledge,
            BaseDefense = blueprint.BaseDefense,
            Regeneration = blueprint.BaseRegeneration,
        };

        var enemy = new Enemy
        {
            Name = blueprint.Name,
            BiomeId = blueprint.BiomeId,
            Stats = stats,
            BaseXp = blueprint.BaseXp,
            BaseGold = blueprint.BaseGold,  
            PassiveEffects = _passiveEffectFactory.GetPassiveEffects(blueprint.PassiveEffectIds),
            NormalBehavior = _behaviorFactory.GetBehavior(blueprint.NormalBehaviorId),
            SpecialBehavior = _behaviorFactory.GetBehavior(blueprint.SpecialBehaviorId),
            BasicAttack = _abilityFactory.GetAbility(blueprint.BasicAttackAbilityId),
            SpecialAbility = _abilityFactory.GetAbility(blueprint.SpecialAbilityId),
            SpecialAbilityCooldown = blueprint.SpecialAbilityCooldown,
            CurrentCooldown = blueprint.SpecialAbilityCooldown,
        };

        return enemy;
    }

    public List<Enemy> CreateEnemies(List<string> blueprintIds)
    {
        var enemies = new List<Enemy>();
        foreach (var id in blueprintIds)
        {
            enemies.Add(CreateEnemy(id));
        }
        return enemies;
    }

    public List<Enemy> SortEnemies(CombatState gameState)
    {
        var blueprintIds = _enemyBlueprints.Values
            .Where(b => b.BiomeId.Equals(gameState.BiomeId, StringComparison.OrdinalIgnoreCase))
            .Select(b => b.Id)
            .ToList();
        if (blueprintIds.Count == 0)
        {
            return new List<Enemy>();
        }

        var difficultyConfig = gameState.GetDifficultyConfig();

        var count = Random.Shared.Next(difficultyConfig.MinEnemyCount, difficultyConfig.MaxEnemyCount + 1);
        var result = new List<Enemy>(count);

        for (var i = 0; i < count; i++)
        {
            var index = Random.Shared.Next(blueprintIds.Count);
            var id = blueprintIds[index];
            result.Add(CreateEnemy(id));
        }

        foreach (var enemy in result)
        {
            enemy.Stats.MaxHealth = (int)(enemy.Stats.MaxHealth * difficultyConfig.EnemyHealthMultiplier);
            enemy.Stats.CurrentHealth = enemy.Stats.MaxHealth;

            enemy.Stats.BaseStrength = (int)(enemy.Stats.BaseStrength * difficultyConfig.EnemyStrengthMultiplier);

            enemy.Stats.BaseDefense = (int)(enemy.Stats.BaseDefense * difficultyConfig.EnemyDefenseMultiplier);
        }

        return result;
    }
}