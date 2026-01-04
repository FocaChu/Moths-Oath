using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IFadingReactor : IEffectReactor
{
    void OnFading(BaseCharacter target, CombatState context);
}
