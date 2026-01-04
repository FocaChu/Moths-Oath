using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Health;

public interface IModifiedHealthReactor : IEffectReactor
{
    void OnHealthModifierApplied(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
