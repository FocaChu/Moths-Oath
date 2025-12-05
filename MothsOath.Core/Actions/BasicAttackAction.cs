using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class BasicAttackAction : BaseAction
{
    public override string Id => "action_basic_attack";

    public override void Execute(ActionContext context)
    {
        int damage = context.Source.Stats.TotalStrength;

        var plan = CreateDamagePlan(context, damage);

        if(CheckTargets(context) || plan.CanProceed == false || plan.FinalDamageAmount <= 0)
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        target.RecieveDamage(context, plan);
        Console.WriteLine($"{context.Source.Name} attacks {target.Name} for {damage} damage.");
    }
}
