using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.StatusEffect.ConcreteStatusEffects;

public class KarmaEffect : BaseStatusEffect, IModifiedHealthReactor
{
    public override string Id { get; set; } = "karma_effect";

    public override string Name { get; set; } = "Karma";

    public override string Description { get; set; } = "Cura metade de toda cura concedida.";

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = true;

    public override bool IsVisible { get; set; } = true;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Positive;

    public int Priority { get; set; } = 1;

    public KarmaEffect(int level, int duration) 
    {
        Level = level;
        Duration = duration;
    }

    public void OnHealthModifierApplied(ActionContext context, HealthModifierPlan plan, BaseCharacter originalTarget)
    {
        if (!IsActive() || originalTarget == context.Source || plan.ModifierType != HealthModifierType.Healing)
            return;

        int healingAmount = plan.FinalValue / 2 + Level;

        var source = context.Source;

        source.ReceivePureHeal(healingAmount);
        Console.WriteLine($"{source.Name} é curado por {healingAmount} devido ao efeito de Karma.");

        base.TickTime(source);
    }
}
