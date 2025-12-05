using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class PowerStrikeAction : BaseAction
{
    public override string Id => "action_power_strike";

    public override void Execute(ActionContext context)
    {
        int damage = (int)(context.Source.Stats.TotalStrength * 2);

        var plan = CreateDamagePlan(context, damage);

        if (CheckTargets(context) || plan.CanProceed == false || plan.FinalDamageAmount <= 0)
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        target.RecieveDamage(context, plan);
        Console.WriteLine($"{context.Source.Name} uses Power Strike on {target.Name} for {damage} damage.");
    }
}
