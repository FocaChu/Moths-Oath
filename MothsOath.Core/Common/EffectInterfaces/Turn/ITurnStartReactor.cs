using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Turn;

public interface ITurnStartReactor
{
    int Priority { get; set; }
    void OnTurnStart(Character target, CombatState context);
}