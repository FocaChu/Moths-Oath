using MothsOath.Core.Common;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.Abilities;

public class SharpCutAction : BaseAction
{
    public override string Id => "action_sharp_cut";

    public override void Execute(ActionContext context)
    {
        int damage = context.Source.Stats.TotalStrength;

        var damagePlan = CreateDamagePlan(context, damage);

        if (CheckTargets(context) || damagePlan.CanProceed == false || damagePlan.FinalDamageAmount <= 0)
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.RecieveDamage(context, damagePlan);

        var bleedingEffect = new BleedingEffect(level: (int)(context.Source.Stats.BaseStrength / 2) + 1, duration: 2);
        var statusEffectPlan = CreateStatusEffectPlan(context, bleedingEffect);

        target.ApplyStatusEffect(context, statusEffectPlan);

        Console.WriteLine($"{context.Source.Name} cut {target.Name} leaving they bleeding!");
    }
}
