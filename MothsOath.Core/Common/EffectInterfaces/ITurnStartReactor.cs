using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface ITurnStartReactor
{
    void OnTurnStart(Character target, CombatState context);
}