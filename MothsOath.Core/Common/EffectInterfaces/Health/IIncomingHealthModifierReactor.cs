using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IIncomingHealthModifierReactor
{
    int Priority { get; set; }
    void ModifyIncomingHealthModifier(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
