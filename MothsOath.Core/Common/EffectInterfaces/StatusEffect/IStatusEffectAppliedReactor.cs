using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IStatusEffectAppliedReactor
{
    void OnStatusEffectApplied(ActionContext context, StatusEffectPlan plan, Character target);
}
