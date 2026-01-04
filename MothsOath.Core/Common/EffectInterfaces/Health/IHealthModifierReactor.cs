using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Health;

public interface IHealthModifierReactor : IEffectReactor
{
    void ReactHealthModified(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
