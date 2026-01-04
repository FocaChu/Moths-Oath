using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IStatusEffectDoneReactor : IEffectReactor
{
    void OnStatusEffectDone(ActionContext context, StatusEffectPlan plan, BaseCharacter target);
}
