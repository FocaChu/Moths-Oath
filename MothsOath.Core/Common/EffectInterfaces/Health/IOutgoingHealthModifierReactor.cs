using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Health;

public interface IOutgoingHealthModifierReactor : IEffectReactor
{
    void ModifyOutgoingHealthModifier(ActionContext context, HealthModifierPlan plan);
}
