using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Health;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class BleedingEffect : BaseStatusEffect, IHealthModifierReactor, ITurnEndReactor
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

        target.ReceivePureDamage(Level);
        Console.WriteLine($"{target.Name} sofre {Level} de dano por sangramento. HP:{target.Stats.CurrentHealth}");
    }

    public void ReactHealthModified(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        if (!IsActive() || plan.FinalValue <= 0 || plan.ModifierType != HealthModifierType.Healing) 
            return;

        Level--;

        Console.WriteLine($"{target.Name} reduziu seu sangramento ao ser curado. Nível atual do sangramento: {Level}");
    }

}
