using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IHealthModifierReactor
{
    int Priority { get; set; }
    void ReactHealthModified(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
