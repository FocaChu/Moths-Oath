namespace MothsOath.Core.Models.Blueprints;

public class EnemyBlueprint
{
    public string Id { get; set; } = "dummy";

    public string Name { get; set; } = string.Empty;

    public int MaxHP { get; set; }

    public int BaseStrength { get; set; } 


    public string BasicAttackAbilityId { get; set; } = string.Empty;

    public string SpecialAbilityId { get; set; } = string.Empty;

    public int SpecialAbilityCooldown { get; set; }
}