using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IOutgoingStatusEffectModifier : IEffectReactor
{
    void ModifyOutgoingStatusEffect(ActionContext context, StatusEffectPlan plan);
}
