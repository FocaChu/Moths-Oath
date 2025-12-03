using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.Interfaces;

public interface IActionPlanModifier
{
    public virtual void ModifyActionPlan(ActionPlan plan, CombatState context)
    {
    }
}
