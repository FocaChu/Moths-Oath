using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class BleedingEffect : BaseStatusEffect, IHealingReceivedReactor, ITurnEndReactor
{
    public override string Id { get; set; } = "bleeding_effect";

    public override string Name { get; set; } = "Sangramento";

    public override string Description { get; set; } = "Causa dano ao final de cada turno e ao ser atacado. Pode ser enfraquecido com cura.";

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = true;

    public override bool IsVisible { get; set; } = true;

    public int Priority { get; set; } = 0;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Negative;

    public BleedingEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }

    public void OnTurnEnd(BaseCharacter target, CombatState context)
    {
        if(!IsActive())
            return;

        target.RecievePureDamage(Level);
        Console.WriteLine($"{target.Name} sofre {Level} de dano por sangramento. HP:{target.Stats.CurrentHealth}");
    }

    public void OnHealingReceived(ActionContext context, HealPlan plan, BaseCharacter target)
    {
        if (!IsActive() || plan.FinalHealAmount <= 0) 
            return;

        Level--;

        Console.WriteLine($"{target.Name} reduziu seu sangramento ao ser curado. Nível atual do sangramento: {Level}");
    }

}
