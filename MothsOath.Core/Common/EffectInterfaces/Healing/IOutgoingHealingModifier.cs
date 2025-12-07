using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IOutgoingHealingModifier
{
    void ModifyOutgoingHealing(ActionContext context, HealPlan plan);
}
