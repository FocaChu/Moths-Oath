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

        var plan = CreateDamagePlan(context, damage);

        if (CheckTargets(context) || plan.CanProceed == false || plan.FinalDamageAmount <= 0)
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        target.RecieveDamage(context, plan);
        Console.WriteLine($"{context.Source.Name} uses Toxic Jab on {target.Name} for {damage} damage.");

        var poisonEffect = new PoisonEffect(level: (int)(context.Source.Stats.BaseKnowledge / 2), duration: 3);

        target.ApplyStatusEffect(poisonEffect);
        Console.WriteLine($"{target.Name} is poisoned!");
    }
}
