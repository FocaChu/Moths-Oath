using MothsOath.Core.Common;
using MothsOath.Core.Entities.Archetypes;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.Abilities;

public class ToxicJabAction : BaseAction
{
    public override string Id => "action_toxic_jab";

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
        Console.WriteLine($"{context.Source.Name} uses Toxic Jab on {target.Name} for {damage} damage.");

        var poisonEffect = new PoisonEffect(level: (int)(context.Source.Stats.BaseKnowledge / 2), duration: 3);
        var poisonPlan = CreateStatusEffectPlan(context, poisonEffect);

        target.ApplyStatusEffect(context, poisonPlan);
        Console.WriteLine($"{target.Name} is poisoned!");
    }
}
