using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Combat;

public interface ITurnStartReactor
{
    int Priority { get; set; }
    void OnTurnStart(BaseCharacter target, CombatState context);
}