using MothsOath.Core.Common;

namespace MothsOath.Core.StatusEffect.Interfaces;

public interface IHealReactor
{
    public virtual void ReactToHeal(HealPlan plan, Character target)
    {
    }
}
