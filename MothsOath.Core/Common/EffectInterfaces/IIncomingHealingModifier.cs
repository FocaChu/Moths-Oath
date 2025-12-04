using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IIncomingHealingModifier
{
    void ModifyIncomingHealing(HealPlan plan, ActionContext context);
}
