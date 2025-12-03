using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class PowerStrikeAction : IAction
{
    public string Id => "action_power_strike";

    public void Execute(ActionContext context)
    {
        var rng = new Random();
        var target = context.Targets[rng.Next(context.Targets.Count)];

        int damage = context.Source.BaseStrength * 2;
        target.TakeDamage(damage, false);
        Console.WriteLine($"{context.Source.Name} uses Power Strike on {target.Name} for {damage} damage.");
    }
}
