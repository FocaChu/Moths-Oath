using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class BasicAttackAbility : IAction
{
    public string Id => "ability_basic_attack";

    public void Execute(ActionContext context)
    {
        int damage = context.Source.BaseStrength;
        context.Target.TakeDamage(damage, false);
        Console.WriteLine($"{context.Source.Name} attacks {context.Target.Name} for {damage} damage.");
    }
}
