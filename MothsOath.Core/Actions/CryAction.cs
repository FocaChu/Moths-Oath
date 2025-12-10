using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class CryAction : BaseAction
{
    public override string Id => "action_cry";

    public override void Execute(ActionContext context)
    {
        if (!ValidadeTargets(context))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.Stats.TemporaryDefense -= (int)(context.Source.Stats.BaseKnowledge /2) + 1;

        Console.WriteLine($"{context.Source.Name} lets out a heartfelt cry on {target.Name}!");
    }
}
