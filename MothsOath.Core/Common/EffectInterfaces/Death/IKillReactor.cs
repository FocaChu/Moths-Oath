using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Death;

public interface IKillReactor : IEffectReactor
{
    void OnKill(ActionContext context, MortuaryPlan plan, BaseCharacter victim);
}
