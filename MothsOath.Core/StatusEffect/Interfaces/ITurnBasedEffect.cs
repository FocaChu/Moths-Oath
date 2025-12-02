using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.Interfaces;

public interface ITurnBasedEffect
{
    public virtual void OnTurnEnd(Character target, CombatState context)
    {
    }
}
