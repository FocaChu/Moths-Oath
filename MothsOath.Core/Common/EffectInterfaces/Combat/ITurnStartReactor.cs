using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Combat;

public interface ITurnStartReactor : IEffectReactor
{
    void OnTurnStart(BaseCharacter target, CombatState context);
}
