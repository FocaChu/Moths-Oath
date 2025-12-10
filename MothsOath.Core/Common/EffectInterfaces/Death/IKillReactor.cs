using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Common.EffectInterfaces.Death;

public interface IKillReactor
{
    int Priority { get; set; }
    void OnKill(ActionContext context, MortuaryPlan plan, BaseCharacter victim);
}
