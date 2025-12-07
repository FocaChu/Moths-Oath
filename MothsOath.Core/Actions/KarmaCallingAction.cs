using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.StatusEffect.ConcreteStatusEffects;
namespace MothsOath.Core.Actions;

public class KarmaCallingAction : BaseAction
{
    public override string Id => "action_karma_calling";

    public override void Execute(ActionContext context)
    {
        if (CheckTargets(context))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        int level = (int)(context.Source.Stats.TotalKnowledge / 3);
        int duration = 3; 
        var karmaEffect = new KarmaEffect(level, duration);

        var statusEffectPlan = CreateStatusEffectPlan(context, karmaEffect);

        target.ApplyStatusEffect(context, statusEffectPlan);

        Console.WriteLine($"{context.Source.Name} invokes a call of karma upon {target.Name}!");
    }
}
