using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Health;

public interface IIncomingHealthModifierReactor : IEffectReactor
{
    void ModifyIncomingHealthModifier(ActionContext context, HealthModifierPlan plan, BaseCharacter target);
}
