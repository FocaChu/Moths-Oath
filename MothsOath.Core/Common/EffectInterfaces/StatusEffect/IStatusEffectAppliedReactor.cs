using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IStatusEffectAppliedReactor
{
    int Priority { get; set; }
    void OnStatusEffectApplied(ActionContext context, StatusEffectPlan plan, Character target);
}
