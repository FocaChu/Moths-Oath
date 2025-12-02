using MothsOath.Core.Entities;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services;

namespace MothsOath.Core.Factories;

public class EnemyFactory
{
    private readonly Dictionary<string, EnemyBlueprint> _enemyBlueprints;
    private readonly ActionFactory _abilityFactory;
    private readonly BehaviorFactory _behaviorFactory;

    public EnemyFactory(ActionFactory abilityFactory, BehaviorFactory behaviorFactory , BlueprintLoader blueprintLoader)
    {
        _abilityFactory = abilityFactory;
        _behaviorFactory = behaviorFactory;
        _enemyBlueprints = blueprintLoader.LoadAllBlueprintsFromFiles<EnemyBlueprint>("Enemies");
    }

    public Enemy CreateEnemy(string blueprintId)
    {
        if (!_enemyBlueprints.TryGetValue(blueprintId, out var blueprint))
        {
            throw new Exception($"Blueprint de inimigo '{blueprintId}' não encontrado!");
        }

        var enemy = new Enemy
        {
            Name = blueprint.Name,
            MaxHealth = blueprint.MaxHealth,
            CurrentHealth = blueprint.MaxHealth,
            BaseStrength = blueprint.BaseStrength,
            NormalBehavior = _behaviorFactory.GetBehavior(blueprint.NormalBehaviorId),
            SpecialBehavior = _behaviorFactory.GetBehavior(blueprint.SpecialBehaviorId),
            BasicAttack = _abilityFactory.GetAbility(blueprint.BasicAttackAbilityId),
            SpecialAbility = _abilityFactory.GetAbility(blueprint.SpecialAbilityId),
            SpecialAbilityCooldown = blueprint.SpecialAbilityCooldown,
            CurrentCooldown = blueprint.SpecialAbilityCooldown,
        };

        return enemy;
    }
}