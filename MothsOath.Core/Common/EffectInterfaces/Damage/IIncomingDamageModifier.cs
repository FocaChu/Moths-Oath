using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Damage;

public interface IIncomingDamageModifier
{
    void ModifyIncomingDamage(ActionContext context, DamagePlan plan, Character target);
}
