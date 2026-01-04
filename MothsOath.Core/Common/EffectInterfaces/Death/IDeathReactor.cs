using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Death;

public interface IDeathReactor : IEffectReactor
{
    void OnDeath(ActionContext context, MortuaryPlan plan, BaseCharacter victim);
}
