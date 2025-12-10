using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces.Combat;

public interface ICombatStartReactor
{
    int Priority { get; set; }
    void OnCombatStart(BaseCharacter target, CombatState context);
}
