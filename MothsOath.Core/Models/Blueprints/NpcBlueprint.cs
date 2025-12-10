using MothsOath.Core.Models.Blueprints.Common;

namespace MothsOath.Core.Models.Blueprints;

public class NpcBlueprint : IBlueprint
{
    public string Id { get; set; } = "dummy";

    public string BiomeId { get; set; } = "forest_biome";

    public string Name { get; set; } = "dummy";

    public string Allegiance { get; set; } = "Enemy";

    public int XpReward { get; set; }

    public int GoldReward { get; set; }

    public int MaxHealth { get; set; }

    public int BaseStrength { get; set; }

    public int BaseKnowledge { get; set; }

    public int BaseDefense { get; set; }

    public int BaseRegeneration { get; set; }

    public List<string> PassiveEffectIds { get; set; } = new List<string>();

    public string NormalBehaviorId { get; set; } = string.Empty;

    public string SpecialBehaviorId { get; set; } = string.Empty;

    public string BasicAttackAbilityId { get; set; } = string.Empty;

    public string SpecialAbilityId { get; set; } = string.Empty;

    public int SpecialAbilityCooldown { get; set; }
}