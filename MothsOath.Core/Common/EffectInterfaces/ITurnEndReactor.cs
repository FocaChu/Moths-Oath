using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface ITurnEndReactor
{
    void OnTurnEnd(Character target, CombatState context);
}
