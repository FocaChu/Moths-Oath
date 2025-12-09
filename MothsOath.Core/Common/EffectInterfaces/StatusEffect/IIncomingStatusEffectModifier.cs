using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IIncomingStatusEffectModifier
{
    int Priority { get; set; }
    void ModifyIncomingStatusEffect(ActionContext context, StatusEffectPlan plan, BaseCharacter target);
}
