using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.Common.Plans;

public class HealthModifierPlan
{
    public HealthModifierType ModifierType { get; set; }

    public int BaseHealthAmount { get; set; } = 0;

    public int FinalValue { get; set; } = 0;

    public bool HasCritical { get; set; } = false;

    public bool CanProceed { get; set; } = true;

    public bool CanCritical { get; set; } = true;

    public bool BypassResistance { get; set; } = false;

    public HealthModifierPlan(int baseHealthAmount, HealthModifierType modifierType)
    {
        BaseHealthAmount = baseHealthAmount;
        FinalValue = baseHealthAmount;
        ModifierType = modifierType;
    }
}
