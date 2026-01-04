using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Combat;

public interface ITurnEndReactor : IEffectReactor
{
    void OnTurnEnd(BaseCharacter target, CombatState context);
}
