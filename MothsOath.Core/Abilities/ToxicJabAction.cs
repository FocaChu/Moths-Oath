using MothsOath.Core.Common;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.Abilities;

public class ToxicJabAction : IAction
{
    public string Id => "action_toxic_jab";

    public void Execute(ActionContext context)
    {
        var rng = new Random();
        var target = context.Targets[rng.Next(context.Targets.Count)];

        int damage = context.Source.BaseStrength;

        target.TakeDamage(damage, false);
        Console.WriteLine($"{context.Source.Name} uses Toxic Jab on {target.Name} for {damage} damage.");

        var poisonEffect = new PoisonEffect(level: (int)(context.Source.BaseKnowledge /2), duration: 3);

        target.ApplyStatusEffect(poisonEffect);
        Console.WriteLine($"{target.Name} is poisoned!");
    }
}
