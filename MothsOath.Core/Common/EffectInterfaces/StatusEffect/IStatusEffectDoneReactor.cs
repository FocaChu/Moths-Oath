using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.StatusEffect;

public interface IStatusEffectDoneReactor
{
    int Priority { get; set; }
    void OnStatusEffectDone(ActionContext context, StatusEffectPlan plan, Character target);
}
