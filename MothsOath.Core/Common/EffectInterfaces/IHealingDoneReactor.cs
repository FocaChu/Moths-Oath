using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IHealingDoneReactor
{
    void OnHealingDone(ActionContext context, HealPlan plan, Character target);
}
