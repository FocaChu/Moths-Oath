using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class CryAction : IAction
{
    public string Id => "action_cry";

    public void Execute(ActionContext context)
    {
        var rng = new Random();
        var target = context.Targets[rng.Next(context.Targets.Count)];

        target.BonusDefense -= (int)(context.Source.BaseStrength /2);

        Console.WriteLine($"{context.Source.Name} lets out a heartfelt cry!");
    }
}
