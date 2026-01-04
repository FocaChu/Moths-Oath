using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Combat;

public interface ICombatEndReactor : IEffectReactor
{
    void OnCombatEnd(CombatState state, BaseCharacter source);
}
