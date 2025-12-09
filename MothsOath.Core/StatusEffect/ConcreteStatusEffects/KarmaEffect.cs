using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.StatusEffect.ConcreteStatusEffects;

public class KarmaEffect : BaseStatusEffect, IHealingDoneReactor
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

    public void OnHealingDone(ActionContext context, HealPlan plan, BaseCharacter originalTarget)
    {
        if (!IsActive() || originalTarget == context.Source)
            return;

        int healingAmount = plan.FinalHealAmount / 2 + Level;

        context.Source.RecievePureHeal(healingAmount);
        Console.WriteLine($"{context.Source.Name} é curado por {healingAmount} devido ao efeito de Karma.");
    }
}
