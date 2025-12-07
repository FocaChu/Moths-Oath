using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IIncomingStatusEffectModifier
{
    void ModifyIncomingStatusEffect(ActionContext context, StatusEffectPlan plan, Character target);
}
