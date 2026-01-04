using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IIncomingStatusEffectModifier : IEffectReactor
{
    void ModifyIncomingStatusEffect(ActionContext context, StatusEffectPlan plan, BaseCharacter target);
}
