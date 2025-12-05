using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class BasicHealAction : BaseAction
{
    public override string Id => "action_heal";

    public override void Execute(ActionContext context)
    {
        int heal = (int)(context.Source.Stats.TotalKnowledge / 2);

        var plan = CreateHealPlan(context, heal);

        if (CheckTargets(context) || plan.CanProceed == false || plan.FinalHealAmount <= 0)
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        target.RecieveHeal(context, plan);

        Console.WriteLine($"{target} get healed!");
    }
}
