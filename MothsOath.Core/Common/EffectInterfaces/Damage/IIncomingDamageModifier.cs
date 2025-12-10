using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Damage;

public interface IIncomingDamageModifier
{
    int Priority { get; set; }
    void ModifyIncomingDamage(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
