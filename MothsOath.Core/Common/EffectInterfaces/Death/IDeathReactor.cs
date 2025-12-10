using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Death;

public interface IDeathReactor
{
    int Priority { get; set; }

    void OnDeath(ActionContext context, BaseCharacter victim);
}
