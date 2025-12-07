using MothsOath.Core.StatusEffect;

namespace MothsOath.Core.Common.Plans;

public class StatusEffectPlan
{
    public BaseStatusEffect StatusEffect { get; set; }

    public bool CanApply { get; set; } = true;

    public StatusEffectPlan(BaseStatusEffect statusEffect, bool canApply)
    {
        StatusEffect = statusEffect;
        CanApply = canApply;
    }
}
