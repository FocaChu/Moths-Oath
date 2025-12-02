using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class BasicAttackAbility : IAction
{
    public string Id => "ability_basic_attack";

    public void Execute(ActionContext context)
    {
        var rng = new Random();
        var target = context.Targets[rng.Next(context.Targets.Count)];

        int damage = context.Source.BaseStrength;
        target.TakeDamage(damage, false);
        Console.WriteLine($"{context.Source.Name} attacks {target.Name} for {damage} damage.");
    }
}
