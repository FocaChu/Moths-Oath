using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Death;

public interface IKillReactor
{
    int Priority { get; set; }
    void OnKill(ActionContext context, BaseCharacter victim);
}
