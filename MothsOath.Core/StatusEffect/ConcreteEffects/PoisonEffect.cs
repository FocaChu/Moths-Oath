using MothsOath.Core.Common;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect.Interfaces;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class PoisonEffect : BaseStatusEffect, ITurnBasedEffect
{
    public override string Id
    {
        get => "poison_effect";
        set { }
    }

    public override string Name
    {
        get => "Veneno";
        set { }
    }

    public override string Description
    {
        get => "Causa dano ao final de cada turno.";
        set { }
    }

    public PoisonEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }

    public void OnTurnEnd(Character target, CombatState context)
    {
        target.TakeDamage(Level, true);
        Console.WriteLine($"{target.Name} sofre {Level} de dano por veneno. HP:{target.CurrentHealth}");
    }
}
