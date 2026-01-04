using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IStatusEffectAppliedReactor : IEffectReactor
{
    void OnStatusEffectApplied(ActionContext context, StatusEffectPlan plan, BaseCharacter target);
}
