using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.StatusEffect.ConcreteStatusEffects;

public class KarmaEffect : BaseStatusEffect, IHealingDoneReactor
{
    public override string Id => "karma_effect";

    public override string Name => "Karma";

    public override string Description => "Cura metade de toda cura concedida.";

    public override bool IsEndless { get; set; } = false;

    public KarmaEffect(int level, int duration) 
    {
        Level = level;
        Duration = duration;
    }

    public void OnHealingDone(ActionContext context, HealPlan plan, Character originalTarget)
    {
        if (!IsActive() || originalTarget == context.Source)
            return;

        int healingAmount = plan.FinalHealAmount / 2 + Level;

        context.Source.RecievePureHeal(healingAmount);
        Console.WriteLine($"{context.Source.Name} é curado por {healingAmount} devido ao efeito de Karma.");
    }
}
