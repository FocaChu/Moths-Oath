using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.Interfaces;

public interface IActionPlanModifier
{
    public virtual void ModifyIntent(ActionPlan plan, CombatState context)
    {
    }
}
