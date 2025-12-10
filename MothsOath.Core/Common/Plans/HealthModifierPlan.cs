namespace MothsOath.Core.Common.Plans;

public class HealthModifierPlan
{
    public int BaseHealthAmount { get; set; } = 0;

    public int FinalValue { get; set; } = 0;

    public bool CanProceed { get; set; } = true;

    public bool CanCritical { get; set; } = true;

    public bool BypassResistance { get; set; } = false;

    public HealthModifierPlan(int baseHealthAmount)
    {
        BaseHealthAmount = baseHealthAmount;
        FinalValue = baseHealthAmount;
    }
}
