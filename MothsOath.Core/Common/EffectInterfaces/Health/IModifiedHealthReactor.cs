using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IModifiedHealthReactor
{
    int Priority { get; set; }
    void OnHealthModifierApplied(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
