using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Turn;

public interface ITurnEndReactor
{
    int Priority { get; set; }
    void OnTurnEnd(Character target, CombatState context);
}
