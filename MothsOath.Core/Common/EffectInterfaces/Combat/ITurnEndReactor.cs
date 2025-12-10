using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Combat;

public interface ITurnEndReactor
{
    int Priority { get; set; }
    void OnTurnEnd(BaseCharacter target, CombatState context);
}
