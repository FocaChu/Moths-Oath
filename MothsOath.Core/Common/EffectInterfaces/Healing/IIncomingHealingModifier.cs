using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IIncomingHealingModifier
{
    void ModifyIncomingHealing(ActionContext context, HealPlan plan, Character target);
}
