using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IStatusEffectRecievedReactor
{
    void OnStatusEffectReceived(ActionContext context, StatusEffectPlan plan, Character target);
}
