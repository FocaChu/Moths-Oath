using MothsOath.Core.Common;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect.Interfaces;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class BleedingEffect : BaseStatusEffect, IHealReactor, ITurnBasedEffect
{
    public override string Id
    {
        get => "bleeding_effect";
        set { }
    }

    public override string Name
    {
        get => "Sangramento";
        set { }
    }

    public override string Description
    {
        get => "Causa dano ao final de cada turno e ao ser atacado. Pode ser enfraquecido com cura.";
        set { }
    }

    public BleedingEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }

    public void OnTurnEnd(Character target, CombatState context)
    {
        if(!IsActive())
            return;

        target.TakeDamage(Level, true);
        Console.WriteLine($"{target.Name} sofre {Level} de dano por sangramento. HP:{target.CurrentHealth}");
    }

    public void ReactToHeal(HealPlan plan, Character target)
    {
        if (!IsActive() || plan.FinalHealAmount <= 0) 
            return;

        Level--;

        Console.WriteLine($"{target.Name} reduziu seu sangramento ao ser curado. Nível atual do sangramento: {Level}");
    }

}
