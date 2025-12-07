using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Healing;

public interface IHealingReceivedReactor
{
    void OnHealingReceived(ActionContext context, HealPlan plan, Character target);
}
