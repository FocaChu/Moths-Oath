using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IOutgoingHealingModifier
{
    int Priority { get; set; }
    void ModifyOutgoingHealing(ActionContext context, HealPlan plan);
}
