using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.Services;
using MothsOath.Core.States;

namespace MothsOath.Core.Factories;

public class NpcFactory
{
    private readonly Dictionary<string, NpcBlueprint> _npcBlueprints;
    private readonly ActionFactory _abilityFactory;
    private readonly BehaviorFactory _behaviorFactory;
    private readonly PassiveEffectFactory _passiveEffectFactory;

    public NpcFactory(ActionFactory abilityFactory, BehaviorFactory behaviorFactory , PassiveEffectFactory passiveEffectFactory, BlueprintCache blueprintCache)
    {
        _abilityFactory = abilityFactory;
        _behaviorFactory = behaviorFactory;
        _passiveEffectFactory = passiveEffectFactory;
        _npcBlueprints = blueprintCache.GetNpcs();
    }

    public CharacterNPC CreateNpc(string blueprintId)
    {
        if (!_npcBlueprints.TryGetValue(blueprintId, out var blueprint))
        {
            throw new Exception($"Blueprint de NPC '{blueprintId}' não encontrado!");
        }

        var npcAllegiance = Allegiance.Neutral;

        if (Enum.TryParse(blueprint.Allegiance, true, out Allegiance allegiance))
        {
            npcAllegiance = allegiance;
        }

        var stats = new Stats
        {
            MaxHealth = blueprint.MaxHealth,
            CurrentHealth = blueprint.MaxHealth,
            BaseStrength = blueprint.BaseStrength,
            BaseKnowledge = blueprint.BaseKnowledge,
            BaseDefense = blueprint.BaseDefense,
            Regeneration = blueprint.BaseRegeneration,
            BaseCriticalChance = blueprint.BaseCriticalChance,
            BaseCriticalDamageMultiplier = blueprint.BaseCriticalDamageMultiplier,
        };

        var npc = new CharacterNPC
        {
            Name = blueprint.Name,
            BiomeId = blueprint.BiomeId,
            Allegiance = npcAllegiance,
            Stats = stats,
            XpReward = blueprint.XpReward,
            GoldReward = blueprint.GoldReward,  
            PassiveEffects = _passiveEffectFactory.GetPassiveEffects(blueprint.PassiveEffectIds),
            NormalBehavior = _behaviorFactory.GetBehavior(blueprint.NormalBehaviorId),
            SpecialBehavior = _behaviorFactory.GetBehavior(blueprint.SpecialBehaviorId),
            BasicAbility = _abilityFactory.GetAbility(blueprint.BasicAttackAbilityId),
            SpecialAbility = _abilityFactory.GetAbility(blueprint.SpecialAbilityId),
            SpecialAbilityCooldown = blueprint.SpecialAbilityCooldown,
            CurrentCooldown = blueprint.SpecialAbilityCooldown,
        };

        return npc;
    }

    public List<BaseCharacter> CreateNPCs(List<string> blueprintIds)
    {
        var enemies = new List<BaseCharacter>();
        foreach (var id in blueprintIds)
        {
            enemies.Add(CreateNpc(id));
        }
        return enemies;
    }

    public List<BaseCharacter> SortEnemies(CombatState gameState)
    {
        var blueprintIds = _npcBlueprints.Values
            .Where(b => b.BiomeId.Equals(gameState.BiomeId, StringComparison.OrdinalIgnoreCase) 
            && b.Allegiance.Equals("Enemy", StringComparison.OrdinalIgnoreCase))
            .Select(b => b.Id)
            .ToList();

        if (blueprintIds.Count == 0)
        {
            return new List<BaseCharacter>();
        }

        var difficultyConfig = gameState.GetDifficultyConfig();

        var count = Random.Shared.Next(difficultyConfig.MinEnemyCount, difficultyConfig.MaxEnemyCount + 1);
        var result = new List<BaseCharacter>(count);

        for (var i = 0; i < count; i++)
        {
            var index = Random.Shared.Next(blueprintIds.Count);
            var id = blueprintIds[index];
            result.Add(CreateNpc(id));
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