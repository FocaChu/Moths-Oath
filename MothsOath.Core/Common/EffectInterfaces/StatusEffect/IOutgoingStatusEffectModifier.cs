using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IOutgoingStatusEffectModifier
{
    int Priority { get; set; }
    void ModifyOutgoingStatusEffect(ActionContext context, StatusEffectPlan plan);
}
