using MothsOath.Core.Common;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect.Interfaces;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class PoisonEffect : BaseStatusEffect, ITurnBasedEffect
{
    public override string Id { get; set; } = "poison_effect";

    public override string Name { get; set; } = "Veneno";

    public override string Description { get; set; } = "Causa dano ao final de cada turno.";

    public override bool IsEndless { get; set; } = false;

    public PoisonEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }


    public void OnTurnEnd(Character target, CombatState context)
    {
        if(!IsActive())
            return;

        target.TakeDamage(Level, true);
        Console.WriteLine($"{target.Name} sofre {Level} de dano por veneno. HP:{target.CurrentHealth}");
    }
}
