using MothsOath.Core.States;

namespace MothsOath.Core.Common.EffectInterfaces;

public interface IFadingReactor
{
    int Priority { get; set; }
    void OnFading(Character target, CombatState context);
}
