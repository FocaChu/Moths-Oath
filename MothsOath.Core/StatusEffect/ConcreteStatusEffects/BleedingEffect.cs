using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class BleedingEffect : BaseStatusEffect, IHealingReceivedReactor, ITurnEndReactor
{
    public override string Id => "bleeding_effect";

    public override string Name => "Sangramento";

    public override string Description => "Causa dano ao final de cada turno e ao ser atacado. Pode ser enfraquecido com cura.";

    public override bool IsEndless { get; set; } = false;

    public BleedingEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }

    public void OnTurnEnd(Character target, CombatState context)
    {
        if(!IsActive())
            return;

        target.RecievePureDamage(Level);
        Console.WriteLine($"{target.Name} sofre {Level} de dano por sangramento. HP:{target.Stats.CurrentHealth}");
    }

    public void OnHealingReceived(HealPlan plan, Character target)
    {
        if (!IsActive() || plan.FinalHealAmount <= 0) 
            return;

        Level--;

        Console.WriteLine($"{target.Name} reduziu seu sangramento ao ser curado. Nível atual do sangramento: {Level}");
    }

}
