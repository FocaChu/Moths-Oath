using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Combat;

public interface ICombatStartReactor : IEffectReactor
{
    void OnCombatStart(BaseCharacter target, CombatState context);
}
