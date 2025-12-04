using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IIncomingDamageModifier
{
    void ModifyIncomingDamage(DamagePlan plan, ActionContext context);
}
