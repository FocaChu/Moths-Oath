using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Damage;

public interface IOutgoingDamageModifier
{
    int Priority { get; set; }
    void ModifyOutgoingDamage(ActionContext context, HealthModifierPlan plan);
}
