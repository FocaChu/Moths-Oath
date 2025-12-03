using MothsOath.Core.Common;

namespace MothsOath.Core.StatusEffect.Interfaces;

public interface IHealModifier
{
    public virtual void ModifierHealthPlan(HealPlan plan, Character target)
    {
    }
}
