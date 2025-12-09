using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IFadingReactor
{
    int Priority { get; set; }
    void OnFading(BaseCharacter target, CombatState context);
}
