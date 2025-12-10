using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IIncomingHealingModifier
{
    int Priority { get; set; }
    void ModifyIncomingHealing(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
