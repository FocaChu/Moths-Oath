using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Damage;

public interface IDamageDealtReactor
{
    void OnDamageDealt(ActionContext context, DamagePlan plan, Character target);
}
