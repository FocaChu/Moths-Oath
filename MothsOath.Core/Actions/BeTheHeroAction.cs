using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.Abilities;

public class BeTheHeroAction : BaseAction
{
    public override string Id => "action_be_the_hero";

    public override void Execute(ActionContext context)
    {
        AssistAction(context);
        AttackAction(context);
    }

    private void AssistAction(ActionContext context)
    {
        var source = context.Source;
        var value = (int)(source.Stats.TotalKnowledge / 2) >= 1 ? (int)(source.Stats.TotalKnowledge / 2) : 2;

        source.Stats.BaseCriticalDamageMultiplier += 0.01f;
        source.Stats.Shield += value;

        var allies = context.BaseTargets.Where(t => t.Allegiance == context.Source.Allegiance && t.Stats.IsAlive).ToList();

        if(allies.Count == 0)
            return;

        var rng = Random.Shared;
        var target = allies[rng.Next(allies.Count)];

        target.ReceivePureHeal(value + target.Stats.Regeneration);
        target.Stats.Shield += value;
        target.Stats.BonusStrength++;
    }

    private void AttackAction(ActionContext context)
    {
        int damage = context.Source.Stats.TotalStrength;

        var plan = new HealthModifierPlan(damage, HealthModifierType.Damage);

        if (context.CanOutgoingModifiers)
            ApplyDamageModifiers(context, plan);

        if (!ValidateTargets(context) || !ValidateDamagePlan(context, plan))
            return;

        if (plan.CanCritical)
            plan = CalculateCriticalValue(context, plan);

        var rng = Random.Shared;
        context.FinalTargets = context.FinalTargets.Where(t => t.Allegiance != context.Source.Allegiance).ToList();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.HandleHealthModifier(context, plan);
        Console.WriteLine($"{context.Source.Name} attacks {target.Name} for {plan.FinalValue} damage.");
    }
}
