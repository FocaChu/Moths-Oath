using MothsOath.Core.Models.Blueprints.Common;

namespace MothsOath.Core.Models.Blueprints;

public class EnemyBlueprint : IBlueprint
{
    public string Id { get; set; } = "dummy";

    public string Name { get; set; } = "dummy";

    public int MaxHealth { get; set; }

    public int BaseStrength { get; set; }

    public int BaseResistance { get; set; }

    public string BasicAttackAbilityId { get; set; } = string.Empty;

    public string SpecialAbilityId { get; set; } = string.Empty;

    public int SpecialAbilityCooldown { get; set; }
}