using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Combat;

public interface ICombatEndReactor
{
    int Priority { get; set; }
    void OnCombatEnd(CombatState state, BaseCharacter source);
}
