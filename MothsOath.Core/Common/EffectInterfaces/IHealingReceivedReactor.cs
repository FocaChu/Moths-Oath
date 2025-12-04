using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IHealingReceivedReactor
{
    void OnHealingReceived(HealPlan plan, Character target);
}
