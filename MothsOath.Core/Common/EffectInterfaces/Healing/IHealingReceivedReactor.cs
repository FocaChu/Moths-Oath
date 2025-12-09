using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IHealingReceivedReactor
{
    int Priority { get; set; }
    void OnHealingReceived(ActionContext context, HealPlan plan, BaseCharacter target);
}
