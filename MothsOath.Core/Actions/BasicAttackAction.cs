using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.Abilities;

public class BasicAttackAction : BaseAction
{
    public override string Id => "action_basic_attack";

    public override void Execute(ActionContext context)
    {
        int damage = context.Source.Stats.TotalStrength;

        var plan = new HealthModifierPlan(damage, HealthModifierType.Damage);

        if (context.CanOutgoingModifiers)
            ApplyDamageModifiers(context, plan);

        if (!ValidateTargets(context) || !ValidateDamagePlan(context, plan))
            return;

        if (plan.CanCritical)
            plan = CalculateCriticalValue(context, plan);

        var target = GameRandom.GetRandomElement(context.FinalTargets);

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.HandleHealthModifier(context, plan);
        Console.WriteLine($"{context.Source.Name} attacks {target.Name} for {plan.FinalValue} damage.");
    }
}