using MothsOath.Core.StatusEffect;

namespace MothsOath.Core.Common.Plans;

public class StatusEffectPlan
{
    public BaseStatusEffect StatusEffect { get; set; }


    public StatusEffectPlan(BaseStatusEffect statusEffect)
    {
        StatusEffect = statusEffect;
    }
}
