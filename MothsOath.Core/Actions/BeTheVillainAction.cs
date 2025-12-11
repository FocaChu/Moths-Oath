using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.Abilities;

public class BeTheVillainAction : BaseAction
{
    public override string Id => "action_be_the_villain";

    public override void Execute(ActionContext context)
    {
        AttackAction(context);
    }


    private void AttackAction(ActionContext context)
    {
        int damage = (int)(context.Source.Stats.TotalStrength * 1.25f);
        
        var plan = new HealthModifierPlan(damage, HealthModifierType.Damage);

        if (context.CanOutgoingModifiers)
            ApplyDamageModifiers(context, plan);

        if (!ValidateTargets(context) || !ValidateDamagePlan(context, plan))
            return;

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
