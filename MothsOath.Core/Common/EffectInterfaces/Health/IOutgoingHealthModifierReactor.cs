using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IOutgoingHealthModifierReactor
{
    int Priority { get; set; }
    void ModifyOutgoingHealthModifier(ActionContext context, HealthModifierPlan plan);
}
