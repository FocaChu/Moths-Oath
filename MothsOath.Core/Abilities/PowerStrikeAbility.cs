using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class PowerStrikeAbility : IAction
{
    public string Id => "ability_power_strike";

    public void Execute(ActionContext context)
    {
        int damage = context.Source.BaseStrength * 2;
        context.Target.TakeDamage(damage, false);
        Console.WriteLine($"{context.Source.Name} uses Power Strike on {context.Target.Name} for {damage} damage.");
    }
}
