using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Damage;

public interface IDamageDealtReactor
{
    int Priority { get; set; }

    void OnDamageDealt(ActionContext context, DamagePlan plan, Character target);
}
