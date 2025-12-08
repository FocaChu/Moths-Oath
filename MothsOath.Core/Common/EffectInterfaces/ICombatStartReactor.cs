using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface ICombatStartReactor
{
    int Priority { get; set; }
    void OnCombatStart(Character target, CombatState context);
}
