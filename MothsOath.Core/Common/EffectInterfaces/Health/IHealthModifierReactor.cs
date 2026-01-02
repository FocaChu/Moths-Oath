using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Health;

public interface IHealthModifierReactor
{
    int Priority { get; set; }
    void ReactHealthModified(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
