using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Abilities;

public class BasicHealAction : BaseAction
{
    public override string Id => "action_heal";

    public override void Execute(ActionContext context)
    {
        int heal = (int)(context.Source.Stats.TotalKnowledge / 2);

        var plan = new HealPlan(heal);

        if (context.CanOutgoingModifiers)
            ApplyHealModifiers(context, plan);

        if (!ValidadeTargets(context) || !ValidateHealPlan(context, plan))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.RecieveHeal(context, plan);

        Console.WriteLine($"{target} get healed!");
    }
}
