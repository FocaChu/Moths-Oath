using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IOutgoingDamageModifier
{
    void ModifyOutgoingDamage(ActionContext context, DamagePlan plan);
}
