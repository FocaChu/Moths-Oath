using MothsOath.Core.Common;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.Abilities;

public class SharpCutAction : BaseAction
{
    public override string Id => "action_sharp_cut";

    public override void Execute(ActionContext context)
    {
        int damage = context.Source.Stats.TotalStrength;

        var plan = CreateDamagePlan(context, damage);

        if (CheckTargets(context) || plan.CanProceed == false || plan.FinalDamageAmount <= 0)
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        target.RecieveDamage(context, plan);

        var bleeding = new BleedingEffect(level: (int)(context.Source.Stats.BaseStrength / 2) + 1, duration: 2);
        target.ApplyStatusEffect(bleeding);

        Console.WriteLine($"{context.Source.Name} cut {target.Name} leaving they bleeding!");
    }
}
