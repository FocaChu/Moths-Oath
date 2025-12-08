using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface ICombatStartReactor
{
    void OnCombatStart(Character target, CombatState context);
}
