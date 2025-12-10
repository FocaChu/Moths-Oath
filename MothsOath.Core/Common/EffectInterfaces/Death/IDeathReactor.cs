using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Death;

public interface IDeathReactor
{
    int Priority { get; set; }

    void OnDeath(ActionContext context, MortuaryPlan plan, BaseCharacter victim);
}
