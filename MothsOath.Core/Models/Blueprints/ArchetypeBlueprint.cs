using MothsOath.Core.Models.Blueprints.Common;

namespace MothsOath.Core.Models.Blueprints;

public class ArchetypeBlueprint : IBlueprint
{
    public string Id { get; set; } = "dummy";

    public string Name { get; set; } = "dummy";

    public string Description { get; set; } = "No description provided.";

    public int BonusHealth { get; set; } = 0;

    public int BonusMana { get; set; } = 0;

    public int BonusStamina { get; set; } = 0;

    public int BonusStrength { get; set; } = 0;

    public int BonusKnowledge { get; set; } = 0;

    public int BonusDefense { get; set; } = 0;

    public int BonusRegeneration { get; set; } = 0;

    public int InitialGold { get; set; } = 0;

    public List<string> PassiveEffectIds { get; set; } = new List<string>();

    public List<string> StartingCardIds { get; set; } = new List<string>();
}
