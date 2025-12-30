using MothsOath.Core.Models.Blueprints.Common;

namespace MothsOath.Core.Models.Blueprints;

public class RaceBlueprint : IBlueprint
{
    public string Id { get; set; } = "dummy";

    public string Name { get; set; } = "dummy";

    public string Description { get; set; } = "No description provided.";

    public string PassiveEffectId { get; set; } = "none";

    public int BaseHealth { get; set; } = 100;

    public int BaseMana { get; set; } = 100;

    public int BaseStamina { get; set; } = 100;

    public int BaseStrength { get; set; } = 10;

    public int BaseKnowledge { get; set; } = 10;

    public int BaseDefense { get; set; } = 1;

    public int BaseRegeneration { get; set; } = 0;

    public int BaseCriticalChance { get; set; } = 5;

    public float BaseCriticalDamageMultiplier { get; set; } = 1.5f;

    public float BonusXpMultiplier { get; set; } = 0;

    public string PassiveDescription { get; set; } = "No passive ability.";

    public List<string> StartingCardIds { get; set; } = new List<string>();
}
