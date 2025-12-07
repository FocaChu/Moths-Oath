using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.StatusEffect.ConcreteEffects;
using MothsOath.Core.StatusEffect.ConcreteStatusEffects;
namespace MothsOath.Core.Actions;

public class KarmaCallingAction : BaseAction
{
    public override string Id => "action_karma_calling";

    public override void Execute(ActionContext context)
    {
        if (!ValidadeTargets(context))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        var karmaEffect = new KarmaEffect((int)(context.Source.Stats.TotalKnowledge / 3), 3);

        var effectPlan = new StatusEffectPlan(karmaEffect);

        if (context.CanOutgoingModifiers)
            ApplyStatusEffectModifiers(context, effectPlan);

        if (!ValidadeTargets(context) || !ValidateStatusEffectPlan(context, effectPlan))
            return;

        target.ApplyStatusEffect(context, effectPlan);

        Console.WriteLine($"{context.Source.Name} invokes a call of karma upon {target.Name}!");
    }
}
