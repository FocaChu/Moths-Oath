namespace MothsOath.Core.Common.Plans;

public class MortuaryPlan
{
    public HealthModifierPlan HealthModifierPlan { get; set; } = null!;

    public int HealthBeforeDeath { get; set; } = 0;

    public int ExcessDamage { get; set; } = 0;
}
