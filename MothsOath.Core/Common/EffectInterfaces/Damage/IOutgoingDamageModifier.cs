using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Damage;

public interface IOutgoingDamageModifier
{
    void ModifyOutgoingDamage(ActionContext context, DamagePlan plan);
}
