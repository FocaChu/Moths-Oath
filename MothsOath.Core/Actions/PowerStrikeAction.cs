using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.Abilities;

public class PowerStrikeAction : BaseAction
{
    public override string Id => "action_power_strike";

    public override void Execute(ActionContext context)
    {
        int damage = (int)(context.Source.Stats.TotalStrength * 1.5);

        var plan = new HealthModifierPlan(damage, HealthModifierType.Damage);

        if (context.CanOutgoingModifiers)
            ApplyDamageModifiers(context, plan);

        if (!ValidateTargets(context) || !ValidateDamagePlan(context, plan))
            return;

        if (plan.CanCritical)
            plan = CalculateCriticalValue(context, plan);

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.ReceiveDamage(context, plan);
        Console.WriteLine($"{context.Source.Name} uses Power Strike on {target.Name} for {damage} damage.");
    }
}
