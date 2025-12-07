using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Turn;

public interface ITurnEndReactor
{
    void OnTurnEnd(Character target, CombatState context);
}
