using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IOutgoingHealingModifier
{
    void ModifyOutgoingHealing(ActionContext context, HealPlan plan);
}
