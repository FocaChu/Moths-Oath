using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Turn;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class PoisonEffect : BaseStatusEffect, ITurnEndReactor
{
    public override string Id { get; set; } = "poison_effect";

    public override string Name { get; set; } = "Veneno";

    public override string Description { get; set; } = "Causa dano ao final de cada turno.";

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = true;

    public override bool IsVisible { get; set; } = true;

    public int Priority { get; set; } = 0;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Negative;

    public PoisonEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }


    public void OnTurnEnd(Character target, CombatState context)
    {
        if(!IsActive())
            return;

        target.RecievePureDamage(Level);
        Console.WriteLine($"{target.Name} sofre {Level} de dano por veneno. HP:{target.Stats.CurrentHealth}");
    }
}
