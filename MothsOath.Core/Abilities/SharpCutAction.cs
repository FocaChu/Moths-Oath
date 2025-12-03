using MothsOath.Core.Common;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.Abilities;

public class SharpCutAction : IAction
{
    public string Id => "action_sharp_cut";

    public void Execute(ActionContext context)
    {
        var rng = new Random();
        var target = context.Targets[rng.Next(context.Targets.Count)];

        target.TakeDamage((context.Source.TotalStrength), false);
        
        var bleeding = new BleedingEffect(level: (int)(context.Source.BaseStrength / 2) + 1, duration: 2);
        target.ApplyStatusEffect(bleeding);

        Console.WriteLine($"{context.Source.Name} cut {target.Name} leaving they bleeding!");
    }
}
