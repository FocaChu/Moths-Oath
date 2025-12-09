using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IHealingDoneReactor
{
    int Priority { get; set; }
    void OnHealingDone(ActionContext context, HealPlan plan, BaseCharacter target);
}
