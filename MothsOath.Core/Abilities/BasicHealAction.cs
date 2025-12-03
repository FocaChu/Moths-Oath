using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class BasicHealAction : IAction
{
    public string Id => "action_heal";

    public void Execute(ActionContext context)
    {
        var rng = new Random();
        var target = context.Targets[rng.Next(context.Targets.Count)];

        var healPlan = new HealPlan(context.Source, (int)(context.Source.BaseKnowledge / 2));

        target.Heal(healPlan);

        Console.WriteLine($"{target} get healed!");
    }
}
