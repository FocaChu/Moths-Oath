using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IDamageReceivedReactor
{
    void OnDamageReceived(ActionContext context, DamagePlan plan, Character target);
}
