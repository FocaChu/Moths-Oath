using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Abilities;

public class BasicAttackAction : BaseAction
{
    public override string Id => "action_basic_attack";

    public override void Execute(ActionContext context)
    {
        int damage = context.Source.Stats.TotalStrength;

        var plan = new DamagePlan(damage, false);

        if(context.CanOutgoingModifiers)
            ApplyDamageModifiers(context, plan);

        if (!ValidadeTargets(context) || !ValidateDamagePlan(context, plan))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.RecieveDamage(context, plan);
        Console.WriteLine($"{context.Source.Name} attacks {target.Name} for {damage} damage.");
    }
}
