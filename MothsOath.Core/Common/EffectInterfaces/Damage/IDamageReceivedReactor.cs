using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Damage;

public interface IDamageReceivedReactor
{
    int Priority { get; set; }
    void OnDamageReceived(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
